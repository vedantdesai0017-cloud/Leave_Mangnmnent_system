namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class LeaveService : ILeaveService
{
    private readonly IEmployeeService _employeeService;
    private readonly List<LeaveType> _leaveTypes = new();
    private readonly List<LeaveBalance> _leaveBalances = new();
    private readonly List<LeaveApplication> _leaveApplications = new();
    private readonly object _lock = new();
    private int _nextLeaveTypeId = 1;
    private int _nextLeaveBalanceId = 1;
    private int _nextLeaveApplicationId = 1;

    public LeaveService(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
        SeedData();
    }

    private void SeedData()
    {
        lock (_lock)
        {
            _leaveTypes.Add(new LeaveType { LeaveTypeId = 1, LeaveName = "Casual Leave", Description = "CL - Casual Leave", AnnualAllocation = 12, IsActive = true });
            _leaveTypes.Add(new LeaveType { LeaveTypeId = 2, LeaveName = "Sick Leave", Description = "SL - Sick Leave", AnnualAllocation = 10, IsActive = true });
            _leaveTypes.Add(new LeaveType { LeaveTypeId = 3, LeaveName = "Paid Leave", Description = "PL - Paid/Earned Leave", AnnualAllocation = 15, IsActive = true });
            _leaveTypes.Add(new LeaveType { LeaveTypeId = 4, LeaveName = "Leave Without Pay", Description = "LWP - Leave Without Pay", AnnualAllocation = 365, IsActive = true });
            _nextLeaveTypeId = 5;

            var year = 2026;
            
            var balances = new List<(int empId, int clUsed, int slUsed, int plUsed)>
            {
                (1, 2, 1, 3),
                (2, 1, 0, 2),
                (3, 3, 2, 4),
                (4, 2, 1, 1),
                (5, 0, 0, 2),
                (6, 4, 2, 3),
                (7, 1, 0, 1)
            };

            foreach (var bal in balances)
            {
                AddSeedBalance(bal.empId, 1, 12, bal.clUsed, year);
                AddSeedBalance(bal.empId, 2, 10, bal.slUsed, year);
                AddSeedBalance(bal.empId, 3, 15, bal.plUsed, year);
                AddSeedBalance(bal.empId, 4, 365, 0, year);
            }

            AddSeedApplication(1, 3, new DateTime(2026, 1, 10), new DateTime(2026, 1, 12), LeaveStatus.Approved, "Family function", "Rajesh Kumar");
            AddSeedApplication(3, 1, new DateTime(2026, 9, 20), new DateTime(2026, 9, 20), LeaveStatus.Pending, "Personal work", null);
            AddSeedApplication(4, 2, new DateTime(2026, 8, 15), new DateTime(2026, 8, 15), LeaveStatus.Rejected, "Fever", "Rajesh Kumar", "Please provide medical certificate");
            AddSeedApplication(6, 1, new DateTime(2026, 7, 5), new DateTime(2026, 7, 8), LeaveStatus.Approved, "Vacation", "Admin User");
            AddSeedApplication(7, 3, new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), LeaveStatus.Cancelled, "Trip cancelled", null);
        }
    }

    private void AddSeedBalance(int empId, int leaveTypeId, int total, int used, int year)
    {
        _leaveBalances.Add(new LeaveBalance
        {
            LeaveBalanceId = _nextLeaveBalanceId++,
            EmployeeId = empId,
            LeaveTypeId = leaveTypeId,
            Year = year,
            TotalLeaves = total,
            UsedLeaves = used
        });
    }

    private void AddSeedApplication(int empId, int leaveTypeId, DateTime start, DateTime end, LeaveStatus status, string reason, string? reviewedBy, string? managerComment = null)
    {
        _leaveApplications.Add(new LeaveApplication
        {
            LeaveApplicationId = _nextLeaveApplicationId++,
            EmployeeId = empId,
            LeaveTypeId = leaveTypeId,
            StartDate = start,
            EndDate = end,
            NumberOfDays = (end - start).Days + 1,
            Reason = reason,
            Status = status,
            AppliedDate = start.AddDays(-5),
            ReviewedBy = reviewedBy,
            ReviewedDate = reviewedBy != null ? start.AddDays(-3) : (DateTime?)null,
            ManagerComment = managerComment
        });
    }

    private LeaveApplication PopulateDetails(LeaveApplication app)
    {
        var emp = _employeeService.GetById(app.EmployeeId);
        if (emp != null)
        {
            app.EmployeeName = emp.FullName;
        }

        var lt = _leaveTypes.FirstOrDefault(l => l.LeaveTypeId == app.LeaveTypeId);
        if (lt != null)
        {
            app.LeaveTypeName = lt.LeaveName;
        }
        
        return app;
    }

    private LeaveBalance PopulateBalanceDetails(LeaveBalance bal)
    {
        var lt = _leaveTypes.FirstOrDefault(l => l.LeaveTypeId == bal.LeaveTypeId);
        if (lt != null)
        {
            bal.LeaveTypeName = lt.LeaveName;
        }
        return bal;
    }

    public List<LeaveType> GetAllLeaveTypes()
    {
        lock (_lock) return _leaveTypes.ToList();
    }

    public List<LeaveType> GetActiveLeaveTypes()
    {
        lock (_lock) return _leaveTypes.Where(l => l.IsActive).ToList();
    }

    public LeaveType? GetLeaveTypeById(int id)
    {
        lock (_lock) return _leaveTypes.FirstOrDefault(l => l.LeaveTypeId == id);
    }

    public void AddLeaveType(LeaveType leaveType)
    {
        lock (_lock)
        {
            leaveType.LeaveTypeId = _nextLeaveTypeId++;
            _leaveTypes.Add(leaveType);
        }
    }

    public void UpdateLeaveType(LeaveType leaveType)
    {
        lock (_lock)
        {
            var index = _leaveTypes.FindIndex(l => l.LeaveTypeId == leaveType.LeaveTypeId);
            if (index != -1) _leaveTypes[index] = leaveType;
        }
    }

    public List<LeaveBalance> GetEmployeeBalances(int employeeId)
    {
        lock (_lock)
        {
            return _leaveBalances.Where(b => b.EmployeeId == employeeId).Select(PopulateBalanceDetails).ToList();
        }
    }

    public LeaveBalance? GetBalance(int employeeId, int leaveTypeId)
    {
        lock (_lock)
        {
            var bal = _leaveBalances.FirstOrDefault(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId);
            return bal != null ? PopulateBalanceDetails(bal) : null;
        }
    }

    public void UpdateBalance(int employeeId, int leaveTypeId, int newTotal)
    {
        lock (_lock)
        {
            var bal = _leaveBalances.FirstOrDefault(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId);
            if (bal != null) bal.TotalLeaves = newTotal;
        }
    }

    public void InitializeBalancesForEmployee(int employeeId)
    {
        lock (_lock)
        {
            var year = DateTime.Now.Year;
            foreach (var lt in _leaveTypes.Where(t => t.IsActive))
            {
                if (!_leaveBalances.Any(b => b.EmployeeId == employeeId && b.LeaveTypeId == lt.LeaveTypeId && b.Year == year))
                {
                    _leaveBalances.Add(new LeaveBalance
                    {
                        LeaveBalanceId = _nextLeaveBalanceId++,
                        EmployeeId = employeeId,
                        LeaveTypeId = lt.LeaveTypeId,
                        Year = year,
                        TotalLeaves = lt.AnnualAllocation,
                        UsedLeaves = 0
                    });
                }
            }
        }
    }

    public List<LeaveApplication> GetAll()
    {
        lock (_lock) return _leaveApplications.Select(PopulateDetails).ToList();
    }

    public List<LeaveApplication> GetByEmployee(int employeeId)
    {
        lock (_lock) return _leaveApplications.Where(a => a.EmployeeId == employeeId).Select(PopulateDetails).ToList();
    }

    public List<LeaveApplication> GetPendingByDepartment(int departmentId)
    {
        lock (_lock)
        {
            var empIds = _employeeService.GetByDepartment(departmentId).Select(e => e.EmployeeId).ToList();
            return _leaveApplications.Where(a => a.Status == LeaveStatus.Pending && empIds.Contains(a.EmployeeId))
                                     .Select(PopulateDetails).ToList();
        }
    }

    public List<LeaveApplication> GetByDepartment(int departmentId)
    {
        lock (_lock)
        {
            var empIds = _employeeService.GetByDepartment(departmentId).Select(e => e.EmployeeId).ToList();
            return _leaveApplications.Where(a => empIds.Contains(a.EmployeeId))
                                     .Select(PopulateDetails).ToList();
        }
    }

    public LeaveApplication? GetById(int id)
    {
        lock (_lock)
        {
            var app = _leaveApplications.FirstOrDefault(a => a.LeaveApplicationId == id);
            return app != null ? PopulateDetails(app) : null;
        }
    }

    public List<LeaveApplication> Search(int? employeeId, int? departmentId, int? leaveTypeId, LeaveStatus? status, DateTime? startDate, DateTime? endDate)
    {
        lock (_lock)
        {
            var query = _leaveApplications.AsQueryable();

            if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId.Value);
            if (departmentId.HasValue)
            {
                var empIds = _employeeService.GetByDepartment(departmentId.Value).Select(e => e.EmployeeId).ToList();
                query = query.Where(a => empIds.Contains(a.EmployeeId));
            }
            if (leaveTypeId.HasValue) query = query.Where(a => a.LeaveTypeId == leaveTypeId.Value);
            if (status.HasValue) query = query.Where(a => a.Status == status.Value);
            if (startDate.HasValue) query = query.Where(a => a.StartDate >= startDate.Value);
            if (endDate.HasValue) query = query.Where(a => a.EndDate <= endDate.Value);

            return query.Select(PopulateDetails).ToList();
        }
    }

    public int CalculateLeaveDays(DateTime startDate, DateTime endDate)
    {
        return (endDate - startDate).Days + 1;
    }

    public bool HasOverlappingLeave(int employeeId, DateTime startDate, DateTime endDate, int? excludeApplicationId = null)
    {
        lock (_lock)
        {
            return _leaveApplications.Any(a => 
                a.EmployeeId == employeeId && 
                a.Status != LeaveStatus.Rejected && 
                a.Status != LeaveStatus.Cancelled &&
                (excludeApplicationId == null || a.LeaveApplicationId != excludeApplicationId) &&
                startDate <= a.EndDate && 
                endDate >= a.StartDate);
        }
    }

    public (bool Success, string Message) ApplyLeave(LeaveApplication application)
    {
        lock (_lock)
        {
            if (application.StartDate > application.EndDate)
                return (false, "Start date cannot be after end date.");

            application.NumberOfDays = CalculateLeaveDays(application.StartDate, application.EndDate);

            if (HasOverlappingLeave(application.EmployeeId, application.StartDate, application.EndDate))
                return (false, "You already have a leave application during this period.");

            var isLwp = application.LeaveTypeId == 4;
            if (!isLwp)
            {
                var balance = GetBalance(application.EmployeeId, application.LeaveTypeId);
                if (balance == null || balance.RemainingLeaves < application.NumberOfDays)
                    return (false, "Insufficient leave balance.");
            }

            application.LeaveApplicationId = _nextLeaveApplicationId++;
            application.Status = LeaveStatus.Pending;
            application.AppliedDate = DateTime.Now;
            _leaveApplications.Add(application);

            return (true, "Leave applied successfully.");
        }
    }

    public (bool Success, string Message) ApproveLeave(int applicationId, string reviewedBy)
    {
        lock (_lock)
        {
            var app = _leaveApplications.FirstOrDefault(a => a.LeaveApplicationId == applicationId);
            if (app == null || app.Status != LeaveStatus.Pending)
                return (false, "Application not found or not in pending state.");

            app.Status = LeaveStatus.Approved;
            app.ReviewedBy = reviewedBy;
            app.ReviewedDate = DateTime.Now;

            var balance = _leaveBalances.FirstOrDefault(b => b.EmployeeId == app.EmployeeId && b.LeaveTypeId == app.LeaveTypeId);
            if (balance != null)
            {
                balance.UsedLeaves += app.NumberOfDays;
            }

            return (true, "Leave approved successfully.");
        }
    }

    public (bool Success, string Message) RejectLeave(int applicationId, string reviewedBy, string comment)
    {
        lock (_lock)
        {
            var app = _leaveApplications.FirstOrDefault(a => a.LeaveApplicationId == applicationId);
            if (app == null || app.Status != LeaveStatus.Pending)
                return (false, "Application not found or not in pending state.");

            app.Status = LeaveStatus.Rejected;
            app.ReviewedBy = reviewedBy;
            app.ReviewedDate = DateTime.Now;
            app.ManagerComment = comment;

            return (true, "Leave rejected successfully.");
        }
    }

    public (bool Success, string Message) CancelLeave(int applicationId, int employeeId)
    {
        lock (_lock)
        {
            var app = _leaveApplications.FirstOrDefault(a => a.LeaveApplicationId == applicationId && a.EmployeeId == employeeId);
            if (app == null || app.Status != LeaveStatus.Pending)
                return (false, "Application not found or cannot be cancelled.");

            app.Status = LeaveStatus.Cancelled;

            return (true, "Leave cancelled successfully.");
        }
    }

    public int GetEmployeesOnLeaveToday()
    {
        lock (_lock)
        {
            var today = DateTime.Today;
            return _leaveApplications.Count(a => a.Status == LeaveStatus.Approved && a.StartDate <= today && a.EndDate >= today);
        }
    }

    public int GetEmployeesOnLeaveTodayByDepartment(int departmentId)
    {
        lock (_lock)
        {
            var today = DateTime.Today;
            var empIds = _employeeService.GetByDepartment(departmentId).Select(e => e.EmployeeId).ToList();
            return _leaveApplications.Count(a => empIds.Contains(a.EmployeeId) && a.Status == LeaveStatus.Approved && a.StartDate <= today && a.EndDate >= today);
        }
    }
}
