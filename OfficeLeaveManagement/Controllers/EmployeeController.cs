using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;
using OfficeLeaveManagement.ViewModels;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Employee", "Manager", "Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveService _leaveService;

        public EmployeeController(IEmployeeService employeeService, ILeaveService leaveService)
        {
            _employeeService = employeeService;
            _leaveService = leaveService;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Employee Dashboard";
            ViewData["ActiveMenu"] = "Dashboard";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var employee = _employeeService.GetById(employeeId);
            if (employee == null) return RedirectToAction("Login", "Account");

            var balances = _leaveService.GetEmployeeBalances(employeeId);
            var leaves = _leaveService.GetByEmployee(employeeId);

            var model = new EmployeeDashboardViewModel
            {
                EmployeeName = employee.FullName,
                EmployeeCode = employee.EmployeeCode,
                DepartmentName = employee.DepartmentName ?? "N/A",
                TotalLeaves = balances.Sum(b => b.TotalLeaves),
                UsedLeaves = balances.Sum(b => b.UsedLeaves),
                RemainingLeaves = balances.Sum(b => b.RemainingLeaves),
                PendingRequests = leaves.Count(l => l.Status == LeaveStatus.Pending),
                ApprovedRequests = leaves.Count(l => l.Status == LeaveStatus.Approved),
                RejectedRequests = leaves.Count(l => l.Status == LeaveStatus.Rejected),
                RecentApplications = leaves.OrderByDescending(l => l.AppliedDate).Take(5).ToList(),
                LeaveBalances = balances
            };

            return View(model);
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "My Profile";
            ViewData["ActiveMenu"] = "Profile";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var employee = _employeeService.GetById(employeeId);
            if (employee == null) return RedirectToAction("Login", "Account");

            var model = new ProfileViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentName = employee.DepartmentName ?? "N/A",
                Role = employee.Role.ToString(),
                JoiningDate = employee.JoiningDate
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(ProfileViewModel model)
        {
            ViewData["Title"] = "My Profile";
            ViewData["ActiveMenu"] = "Profile";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var employee = _employeeService.GetById(employeeId);
            if (employee == null) return RedirectToAction("Login", "Account");

            // Only update allowed fields
            employee.FullName = model.FullName;
            employee.Phone = model.Phone;
            _employeeService.Update(employee);

            // Update session name
            HttpContext.Session.SetString("UserName", model.FullName);

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        public IActionResult Balance()
        {
            ViewData["Title"] = "Leave Balance";
            ViewData["ActiveMenu"] = "Balance";

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var balances = _leaveService.GetEmployeeBalances(employeeId);

            return View(balances);
        }
    }
}
