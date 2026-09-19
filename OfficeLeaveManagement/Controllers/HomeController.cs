using Microsoft.AspNetCore.Mvc;

namespace OfficeLeaveManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (!string.IsNullOrEmpty(role))
            {
                return role switch
                {
                    "Admin" => RedirectToAction("Dashboard", "Admin"),
                    "Manager" => RedirectToAction("Dashboard", "Manager"),
                    _ => RedirectToAction("Dashboard", "Employee")
                };
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
