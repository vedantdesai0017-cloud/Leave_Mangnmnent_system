using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;
using OfficeLeaveManagement.ViewModels;
using OfficeLeaveManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveService _leaveService;
        private readonly IAuditService _auditService;

        public AdminController(IEmployeeService employeeService, ILeaveService leaveService, IAuditService auditService)
        {
            _employeeService = employeeService;
            _leaveService = leaveService;
            _auditService = auditService;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard";
            ViewData["ActiveMenu"] = "Dashboard";

            var employees = _employeeService.GetActive();
            var departments = _employeeService.GetActiveDepartments();
            var allLeaves = _leaveService.GetAll();

            // Status distribution
            var statusLabels = new List<string> { "Pending", "Approved", "Rejected", "Cancelled" };
            var statusCounts = new List<int>
            {
                allLeaves.Count(l => l.Status == LeaveStatus.Pending),
                allLeaves.Count(l => l.Status == LeaveStatus.Approved),
                allLeaves.Count(l => l.Status == LeaveStatus.Rejected),
                allLeaves.Count(l => l.Status == LeaveStatus.Cancelled)
            };

            // Department-wise leave counts
            var deptLabels = departments.Select(d => d.DepartmentName).ToList();
            var deptCounts = departments.Select(d =>
                allLeaves.Count(l =>
                {
                    var emp = _employeeService.GetById(l.EmployeeId);
                    return emp != null && emp.DepartmentId == d.DepartmentId;
                })).ToList();

            // Monthly leave counts for current year
            var monthlyLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var monthlyCounts = Enumerable.Range(1, 12).Select(m =>
                allLeaves.Count(l => l.AppliedDate.Year == DateTime.Now.Year && l.AppliedDate.Month == m)).ToList();

            var model = new AdminDashboardViewModel
            {
                TotalEmployees = employees.Count,
                TotalDepartments = departments.Count,
                PendingRequests = statusCounts[0],
                ApprovedRequests = statusCounts[1],
                RejectedRequests = statusCounts[2],
                EmployeesOnLeave = _leaveService.GetEmployeesOnLeaveToday(),
                StatusLabels = statusLabels,
                StatusCounts = statusCounts,
                DepartmentLabels = deptLabels,
                DepartmentLeaveCounts = deptCounts,
                MonthlyLabels = monthlyLabels,
                MonthlyLeaveCounts = monthlyCounts,
                RecentApplications = allLeaves.OrderByDescending(l => l.AppliedDate).Take(10).ToList()
            };

            return View(model);
        }

        public IActionResult Employees(string? search, int? departmentId, string? role)
        {
            ViewData["Title"] = "Employee Management";
            ViewData["ActiveMenu"] = "Employees";

            var employees = _employeeService.Search(search, departmentId, role);
            ViewBag.Departments = _employeeService.GetActiveDepartments();
            ViewBag.SearchTerm = search;
            ViewBag.DepartmentFilter = departmentId;
            ViewBag.RoleFilter = role;

            return View(employees);
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            ViewData["Title"] = "Add Employee";
            ViewData["ActiveMenu"] = "Employees";

            var model = new EmployeeViewModel
            {
                Departments = _employeeService.GetActiveDepartments(),
                JoiningDate = DateTime.Today,
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmployee(EmployeeViewModel model)
        {
            ViewData["Title"] = "Add Employee";
            ViewData["ActiveMenu"] = "Employees";
            model.Departments = _employeeService.GetActiveDepartments();

            if (!ModelState.IsValid)
                return View(model);

            // Check for duplicate email
            var existing = _employeeService.GetByEmail(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("Email", "An employee with this email already exists.");
                return View(model);
            }

            var employee = new Employee
            {
                EmployeeCode = model.EmployeeCode,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                DepartmentId = model.DepartmentId,
                Role = model.Role,
                JoiningDate = model.JoiningDate,
                IsActive = model.IsActive
            };

            _employeeService.Add(employee);
            _leaveService.InitializeBalancesForEmployee(employee.EmployeeId);

            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            _auditService.Log(userName, "CREATE_EMPLOYEE", $"Created employee: {employee.FullName} ({employee.EmployeeCode})");

            TempData["SuccessMessage"] = "Employee created successfully.";
            return RedirectToAction("Employees");
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            ViewData["Title"] = "Edit Employee";
            ViewData["ActiveMenu"] = "Employees";

            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction("Employees");
            }

            var model = new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentId = employee.DepartmentId,
                Role = employee.Role,
                JoiningDate = employee.JoiningDate,
                IsActive = employee.IsActive,
                Departments = _employeeService.GetActiveDepartments()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmployee(EmployeeViewModel model)
        {
            ViewData["Title"] = "Edit Employee";
            ViewData["ActiveMenu"] = "Employees";
            model.Departments = _employeeService.GetActiveDepartments();

            if (!ModelState.IsValid)
                return View(model);

            var employee = _employeeService.GetById(model.EmployeeId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction("Employees");
            }

            employee.FullName = model.FullName;
            employee.Phone = model.Phone;
            employee.DepartmentId = model.DepartmentId;
            employee.Role = model.Role;
            employee.JoiningDate = model.JoiningDate;
            employee.IsActive = model.IsActive;

            _employeeService.Update(employee);

            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            _auditService.Log(userName, "UPDATE_EMPLOYEE", $"Updated employee: {employee.FullName} ({employee.EmployeeCode})");

            TempData["SuccessMessage"] = "Employee updated successfully.";
            return RedirectToAction("Employees");
        }

        public IActionResult LeaveBalances(string? search)
        {
            ViewData["Title"] = "Leave Balances";
            ViewData["ActiveMenu"] = "LeaveBalances";

            var employees = _employeeService.GetActive();
            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e =>
                    e.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.EmployeeCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var balanceViewModels = employees.Select(e =>
            {
                var balances = _leaveService.GetEmployeeBalances(e.EmployeeId);
                return new LeaveBalanceViewModel
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.FullName,
                    EmployeeCode = e.EmployeeCode,
                    DepartmentName = e.DepartmentName ?? "N/A",
                    Balances = balances.Select(b => new LeaveBalanceItem
                    {
                        LeaveBalanceId = b.LeaveBalanceId,
                        LeaveTypeName = b.LeaveTypeName ?? "Unknown",
                        TotalLeaves = b.TotalLeaves,
                        UsedLeaves = b.UsedLeaves,
                        RemainingLeaves = b.RemainingLeaves,
                        Year = b.Year
                    }).ToList()
                };
            }).ToList();

            ViewBag.SearchTerm = search;
            return View(balanceViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateBalance(int employeeId, int leaveTypeId, int newTotal)
        {
            _leaveService.UpdateBalance(employeeId, leaveTypeId, newTotal);

            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            var employee = _employeeService.GetById(employeeId);
            _auditService.Log(userName, "UPDATE_BALANCE",
                $"Updated leave balance for {employee?.FullName}: LeaveTypeId={leaveTypeId}, NewTotal={newTotal}");

            TempData["SuccessMessage"] = "Leave balance updated successfully.";
            return RedirectToAction("LeaveBalances");
        }

        public IActionResult AllLeaves(int? employeeId, int? departmentId, int? leaveTypeId, string? status, DateTime? startDate, DateTime? endDate)
        {
            ViewData["Title"] = "All Leave Requests";
            ViewData["ActiveMenu"] = "AllLeaves";

            LeaveStatus? leaveStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsedStatus))
                leaveStatus = parsedStatus;

            var leaves = _leaveService.Search(employeeId, departmentId, leaveTypeId, leaveStatus, startDate, endDate);

            ViewBag.Employees = _employeeService.GetActive();
            ViewBag.Departments = _employeeService.GetActiveDepartments();
            ViewBag.LeaveTypes = _leaveService.GetActiveLeaveTypes();
            ViewBag.EmployeeFilter = employeeId;
            ViewBag.DepartmentFilter = departmentId;
            ViewBag.LeaveTypeFilter = leaveTypeId;
            ViewBag.StatusFilter = status;
            ViewBag.StartDateFilter = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDateFilter = endDate?.ToString("yyyy-MM-dd");

            return View(leaves.OrderByDescending(l => l.AppliedDate).ToList());
        }
    }
}
