using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Employee code is required.")]
    [StringLength(20, ErrorMessage = "Employee code cannot exceed 20 characters.")]
    [Display(Name = "Employee Code")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Display(Name = "Department Name")]
    public string? DepartmentName { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public UserRole Role { get; set; } = UserRole.Employee;

    [Required(ErrorMessage = "Joining date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
