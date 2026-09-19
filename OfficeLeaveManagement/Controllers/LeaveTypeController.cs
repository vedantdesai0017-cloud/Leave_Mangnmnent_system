using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Admin")]
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IAuditService _auditService;

        public LeaveTypeController(ILeaveService leaveService, IAuditService auditService)
        {
            _leaveService = leaveService;
            _auditService = auditService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Leave Type Management";
            ViewData["ActiveMenu"] = "LeaveTypes";
            var leaveTypes = _leaveService.GetAllLeaveTypes();
            return View(leaveTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Add Leave Type";
            ViewData["ActiveMenu"] = "LeaveTypes";
            return View(new LeaveType { IsActive = true, AnnualAllocation = 12 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LeaveType model)
        {
            ViewData["Title"] = "Add Leave Type";
            ViewData["ActiveMenu"] = "LeaveTypes";

            if (!ModelState.IsValid) return View(model);

            _leaveService.AddLeaveType(model);
            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            _auditService.Log(userName, "CREATE_LEAVE_TYPE", $"Created leave type: {model.LeaveName} (Allocation: {model.AnnualAllocation})");
            TempData["SuccessMessage"] = "Leave type created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit Leave Type";
            ViewData["ActiveMenu"] = "LeaveTypes";

            var lt = _leaveService.GetLeaveTypeById(id);
            if (lt == null)
            {
                TempData["ErrorMessage"] = "Leave type not found.";
                return RedirectToAction("Index");
            }
            return View(lt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(LeaveType model)
        {
            ViewData["Title"] = "Edit Leave Type";
            ViewData["ActiveMenu"] = "LeaveTypes";

            if (!ModelState.IsValid) return View(model);

            _leaveService.UpdateLeaveType(model);
            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            _auditService.Log(userName, "UPDATE_LEAVE_TYPE", $"Updated leave type: {model.LeaveName}");
            TempData["SuccessMessage"] = "Leave type updated successfully.";
            return RedirectToAction("Index");
        }
    }
}
