using Microsoft.AspNetCore.Mvc;
using OfficeLeaveManagement.Filters;
using OfficeLeaveManagement.Services;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.Controllers
{
    [AuthorizeRole("Admin")]
    public class DepartmentController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAuditService _auditService;

        public DepartmentController(IEmployeeService employeeService, IAuditService auditService)
        {
            _employeeService = employeeService;
            _auditService = auditService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Department Management";
            ViewData["ActiveMenu"] = "Departments";
            var departments = _employeeService.GetAllDepartments();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Add Department";
            ViewData["ActiveMenu"] = "Departments";
            return View(new Department { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department model)
        {
            ViewData["Title"] = "Add Department";
            ViewData["ActiveMenu"] = "Departments";

            if (!ModelState.IsValid) return View(model);

            _employeeService.AddDepartment(model);
            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            _auditService.Log(userName, "CREATE_DEPARTMENT", $"Created department: {model.DepartmentName}");
            TempData["SuccessMessage"] = "Department created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit Department";
            ViewData["ActiveMenu"] = "Departments";

            var dept = _employeeService.GetDepartmentById(id);
            if (dept == null)
            {
                TempData["ErrorMessage"] = "Department not found.";
                return RedirectToAction("Index");
            }
            return View(dept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department model)
        {
            ViewData["Title"] = "Edit Department";
            ViewData["ActiveMenu"] = "Departments";

            if (!ModelState.IsValid) return View(model);

            _employeeService.UpdateDepartment(model);
            var userName = HttpContext.Session.GetString("UserName") ?? "Admin";
            _auditService.Log(userName, "UPDATE_DEPARTMENT", $"Updated department: {model.DepartmentName}");
            TempData["SuccessMessage"] = "Department updated successfully.";
            return RedirectToAction("Index");
        }
    }
}
