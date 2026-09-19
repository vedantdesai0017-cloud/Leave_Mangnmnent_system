using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Admin", "Manager")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Reports";
            ViewData["ActiveMenu"] = "Reports";

            var model = _reportService.GetReportData();
            return View(model);
        }
    }
}
