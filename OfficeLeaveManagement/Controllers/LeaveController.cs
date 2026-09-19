using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;
using OfficeLeaveManagement.ViewModels;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Employee", "Manager", "Admin")]
    public class LeaveController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;
        private readonly IAuditService _auditService;

        public LeaveController(ILeaveService leaveService, IEmployeeService employeeService, IAuditService auditService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
            _auditService = auditService;
        }

        [HttpGet]
        public IActionResult Apply()
        {
            ViewData["Title"] = "Apply Leave";
            ViewData["ActiveMenu"] = "ApplyLeave";

            var model = new ApplyLeaveViewModel
            {
                AvailableLeaveTypes = _leaveService.GetActiveLeaveTypes(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(ApplyLeaveViewModel model)
        {
            ViewData["Title"] = "Apply Leave";
            ViewData["ActiveMenu"] = "ApplyLeave";
            model.AvailableLeaveTypes = _leaveService.GetActiveLeaveTypes();

            if (!ModelState.IsValid)
                return View(model);

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";
            var employee = _employeeService.GetById(employeeId);
            if (employee == null) return RedirectToAction("Login", "Account");

            // Server-side date validation
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be on or after start date.");
                return View(model);
            }

            var numberOfDays = _leaveService.CalculateLeaveDays(model.StartDate, model.EndDate);

            var application = new LeaveApplication
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName,
                LeaveTypeId = model.LeaveTypeId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                NumberOfDays = numberOfDays,
                Reason = model.Reason,
                Status = LeaveStatus.Pending,
                AppliedDate = DateTime.Now
            };

            var result = _leaveService.ApplyLeave(application);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            _auditService.Log(userName, "APPLY_LEAVE",
                $"{userName} applied for leave from {model.StartDate:dd MMM yyyy} to {model.EndDate:dd MMM yyyy} ({numberOfDays} days)");

            TempData["SuccessMessage"] = "Leave application submitted successfully.";
            return RedirectToAction("MyLeaves");
        }

        public IActionResult MyLeaves()
        {
            ViewData["Title"] = "My Leaves";
            ViewData["ActiveMenu"] = "MyLeaves";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var leaves = _leaveService.GetByEmployee(employeeId)
                .OrderByDescending(l => l.AppliedDate).ToList();

            return View(leaves);
        }

        public IActionResult Details(int id)
        {
            ViewData["Title"] = "Leave Details";
            ViewData["ActiveMenu"] = "MyLeaves";

            var application = _leaveService.GetById(id);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Leave application not found.";
                return RedirectToAction("MyLeaves");
            }

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";

            // Employees can only view their own leaves
            if (userRole == "Employee" && application.EmployeeId != employeeId)
            {
                TempData["ErrorMessage"] = "You can only view your own leave applications.";
                return RedirectToAction("MyLeaves");
            }

            var employee = _employeeService.GetById(application.EmployeeId);
            var leaveType = _leaveService.GetLeaveTypeById(application.LeaveTypeId);
            var balance = _leaveService.GetBalance(application.EmployeeId, application.LeaveTypeId);

            var model = new LeaveDetailsViewModel
            {
                Application = application,
                EmployeeName = employee?.FullName ?? "Unknown",
                EmployeeCode = employee?.EmployeeCode ?? "N/A",
                DepartmentName = employee?.DepartmentName ?? "N/A",
                LeaveTypeName = leaveType?.LeaveName ?? "Unknown",
                LeaveBalanceRemaining = balance?.RemainingLeaves ?? 0,
                CanCancel = application.Status == LeaveStatus.Pending && application.EmployeeId == employeeId,
                CanApproveReject = application.Status == LeaveStatus.Pending &&
                    (userRole == "Admin" || userRole == "Manager")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";

            var result = _leaveService.CancelLeave(id, employeeId);

            if (result.Success)
            {
                _auditService.Log(userName, "CANCEL_LEAVE", $"{userName} cancelled leave application #{id}");
                TempData["SuccessMessage"] = "Leave application cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("MyLeaves");
        }
    }
}
