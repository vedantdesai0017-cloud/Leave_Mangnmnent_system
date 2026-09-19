using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.Models;

public class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    public UserRole Role { get; set; } = UserRole.Employee;

    [Display(Name = "Employee ID")]
    public int EmployeeId { get; set; }
}
