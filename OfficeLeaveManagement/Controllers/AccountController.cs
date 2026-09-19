using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.ViewModels;
using OfficeLeaveManagement.Services;

namespace OfficeLeaveManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAuditService _auditService;

        public AccountController(IEmployeeService employeeService, IAuditService auditService)
        {
            _employeeService = employeeService;
            _auditService = auditService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in, redirect to dashboard
            var role = HttpContext.Session.GetString("UserRole");
            if (!string.IsNullOrEmpty(role))
            {
                return RedirectToDashboard(role);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _employeeService.ValidateLogin(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Set session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetInt32("EmployeeId", user.EmployeeId);

            _auditService.Log(user.FullName, "LOGIN", $"{user.FullName} logged in as {user.Role}");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDashboard(user.Role.ToString());
        }

        public IActionResult Logout()
        {
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";
            _auditService.Log(userName, "LOGOUT", $"{userName} logged out");
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                _ => RedirectToAction("Dashboard", "Employee")
            };
        }
    }
}
