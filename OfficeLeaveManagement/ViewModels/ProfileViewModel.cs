using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.ViewModels;

public class ProfileViewModel
{
    public int EmployeeId { get; set; }

    [Display(Name = "Employee Code")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [Display(Name = "Department")]
    public string DepartmentName { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; }
}
