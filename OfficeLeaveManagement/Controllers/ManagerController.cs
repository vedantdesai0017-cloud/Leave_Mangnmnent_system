using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;
using OfficeLeaveManagement.ViewModels;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Manager", "Admin")]
    public class ManagerController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;
        private readonly IAuditService _auditService;

        public ManagerController(ILeaveService leaveService, IEmployeeService employeeService, IAuditService auditService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
            _auditService = auditService;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Manager Dashboard";
            ViewData["ActiveMenu"] = "Dashboard";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var manager = _employeeService.GetById(employeeId);
            if (manager == null) return RedirectToAction("Login", "Account");

            var departmentId = manager.DepartmentId;
            var departmentEmployees = _employeeService.GetByDepartment(departmentId);
            var pendingLeaves = _leaveService.GetPendingByDepartment(departmentId);
            var allDeptLeaves = _leaveService.GetByDepartment(departmentId);

            // Get department-wise stats for charts
            var departments = _employeeService.GetActiveDepartments();
            var deptLabels = departments.Select(d => d.DepartmentName).ToList();
            var deptCounts = departments.Select(d => _leaveService.GetByDepartment(d.DepartmentId).Count).ToList();

            var model = new ManagerDashboardViewModel
            {
                ManagerName = manager.FullName,
                DepartmentName = manager.DepartmentName ?? "N/A",
                PendingApprovals = pendingLeaves.Count,
                EmployeesOnLeave = _leaveService.GetEmployeesOnLeaveTodayByDepartment(departmentId),
                ApprovedRequests = allDeptLeaves.Count(l => l.Status == LeaveStatus.Approved),
                RejectedRequests = allDeptLeaves.Count(l => l.Status == LeaveStatus.Rejected),
                TotalEmployees = departmentEmployees.Count,
                PendingLeaveApplications = pendingLeaves.OrderByDescending(l => l.AppliedDate).ToList(),
                DepartmentLabels = deptLabels,
                DepartmentLeaveCounts = deptCounts
            };

            return View(model);
        }

        public IActionResult PendingRequests()
        {
            ViewData["Title"] = "Pending Requests";
            ViewData["ActiveMenu"] = "PendingRequests";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var manager = _employeeService.GetById(employeeId);
            if (manager == null) return RedirectToAction("Login", "Account");

            // For manager, show pending from their department
            // For admin, show all pending
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";
            List<LeaveApplication> pending;
            if (userRole == "Admin")
                pending = _leaveService.GetAll().Where(l => l.Status == LeaveStatus.Pending).ToList();
            else
                pending = _leaveService.GetPendingByDepartment(manager.DepartmentId);

            return View(pending.OrderByDescending(l => l.AppliedDate).ToList());
        }

        public IActionResult LeaveRequests()
        {
            ViewData["Title"] = "All Leave Requests";
            ViewData["ActiveMenu"] = "LeaveRequests";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var manager = _employeeService.GetById(employeeId);
            if (manager == null) return RedirectToAction("Login", "Account");

            var userRole = HttpContext.Session.GetString("UserRole") ?? "";
            List<LeaveApplication> leaves;
            if (userRole == "Admin")
                leaves = _leaveService.GetAll();
            else
                leaves = _leaveService.GetByDepartment(manager.DepartmentId);

            return View(leaves.OrderByDescending(l => l.AppliedDate).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int applicationId)
        {
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";
            var result = _leaveService.ApproveLeave(applicationId, userName);

            if (result.Success)
            {
                _auditService.Log(userName, "APPROVE_LEAVE", $"{userName} approved leave application #{applicationId}");
                TempData["SuccessMessage"] = "Leave request approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("PendingRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int applicationId, string managerComment)
        {
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";

            if (string.IsNullOrWhiteSpace(managerComment))
            {
                TempData["ErrorMessage"] = "Please provide a reason for rejection.";
                return RedirectToAction("PendingRequests");
            }

            var result = _leaveService.RejectLeave(applicationId, userName, managerComment);

            if (result.Success)
            {
                _auditService.Log(userName, "REJECT_LEAVE", $"{userName} rejected leave application #{applicationId}: {managerComment}");
                TempData["SuccessMessage"] = "Leave request rejected.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("PendingRequests");
        }

        public IActionResult EmployeeHistory()
        {
            ViewData["Title"] = "Employee History";
            ViewData["ActiveMenu"] = "EmployeeHistory";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var manager = _employeeService.GetById(employeeId);
            if (manager == null) return RedirectToAction("Login", "Account");

            var userRole = HttpContext.Session.GetString("UserRole") ?? "";
            List<Employee> employees;
            if (userRole == "Admin")
                employees = _employeeService.GetActive();
            else
                employees = _employeeService.GetByDepartment(manager.DepartmentId);

            // Build a list of employee history data
            var historyData = employees.Select(e => new {
                Employee = e,
                Leaves = _leaveService.GetByEmployee(e.EmployeeId),
                Balances = _leaveService.GetEmployeeBalances(e.EmployeeId)
            }).ToList();

            ViewBag.HistoryData = historyData;
            return View(employees);
        }
    }
}
