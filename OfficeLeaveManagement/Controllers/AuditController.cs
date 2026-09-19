using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Admin")]
    public class AuditController : Controller
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public IActionResult Index(string? userId, string? action, string? searchTerm)
        {
            ViewData["Title"] = "Audit Logs";
            ViewData["ActiveMenu"] = "AuditLogs";

            var logs = _auditService.Search(userId, action, searchTerm);
            
            ViewBag.UserFilter = userId;
            ViewBag.ActionFilter = action;
            ViewBag.SearchTerm = searchTerm;

            return View(logs.OrderByDescending(l => l.Timestamp).ToList());
        }
    }
}
