using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.ViewModels;

public class DepartmentViewModel
{
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Employee Count")]
    public int EmployeeCount { get; set; }
}
