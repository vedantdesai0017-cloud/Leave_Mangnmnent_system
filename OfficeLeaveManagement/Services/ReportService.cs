namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

public class ReportService : IReportService
{
    private readonly ILeaveService _leaveService;
    private readonly IEmployeeService _employeeService;
    private readonly object _lock = new();

    public ReportService(ILeaveService leaveService, IEmployeeService employeeService)
    {
        _leaveService = leaveService;
        _employeeService = employeeService;
    }

    public ReportViewModel GetReportData()
    {
        lock (_lock)
        {
            var allLeaves = _leaveService.GetAll();
            var allEmployees = _employeeService.GetAll();
            var allDepartments = _employeeService.GetAllDepartments();

            var currentYear = DateTime.Now.Year;
            
            var monthlyCounts = new int[12];
            var approvedCurrentYear = allLeaves.Where(l => l.Status == Models.LeaveStatus.Approved && l.StartDate.Year == currentYear).ToList();
            
            foreach (var leave in approvedCurrentYear)
            {
                monthlyCounts[leave.StartDate.Month - 1]++;
            }
            
            var months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            var deptLabels = new List<string>();
            var deptCounts = new List<int>();
            foreach (var dept in allDepartments)
            {
                deptLabels.Add(dept.DepartmentName);
                var empIdsInDept = allEmployees.Where(e => e.DepartmentId == dept.DepartmentId).Select(e => e.EmployeeId).ToList();
                deptCounts.Add(allLeaves.Count(l => empIdsInDept.Contains(l.EmployeeId)));
            }

            var statusLabels = Enum.GetNames(typeof(Models.LeaveStatus)).ToList();
            var statusCounts = new List<int>();
            foreach (var status in Enum.GetValues(typeof(Models.LeaveStatus)).Cast<Models.LeaveStatus>())
            {
                statusCounts.Add(allLeaves.Count(l => l.Status == status));
            }

            var empStats = new List<EmployeeLeaveStatItem>();
            foreach (var emp in allEmployees)
            {
                var empLeaves = allLeaves.Where(l => l.EmployeeId == emp.EmployeeId).ToList();
                empStats.Add(new EmployeeLeaveStatItem
                {
                    EmployeeName = emp.FullName,
                    DepartmentName = emp.DepartmentName ?? "Unknown",
                    TotalApplied = empLeaves.Count,
                    Approved = empLeaves.Count(l => l.Status == Models.LeaveStatus.Approved),
                    Rejected = empLeaves.Count(l => l.Status == Models.LeaveStatus.Rejected),
                    Pending = empLeaves.Count(l => l.Status == Models.LeaveStatus.Pending),
                    Cancelled = empLeaves.Count(l => l.Status == Models.LeaveStatus.Cancelled)
                });
            }

            return new ReportViewModel
            {
                MonthlyLabels = months,
                MonthlyLeaveCounts = monthlyCounts.ToList(),
                DepartmentLabels = deptLabels,
                DepartmentLeaveCounts = deptCounts,
                StatusLabels = statusLabels,
                StatusCounts = statusCounts,
                EmployeeLeaveStats = empStats.OrderByDescending(e => e.TotalApplied).ToList()
            };
        }
    }
}
