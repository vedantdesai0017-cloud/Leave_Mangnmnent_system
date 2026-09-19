using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.Models;

public class LeaveApplication
{
    public int LeaveApplicationId { get; set; }

    [Required]
    [Display(Name = "Employee ID")]
    public int EmployeeId { get; set; }

    [Display(Name = "Employee Name")]
    public string? EmployeeName { get; set; }

    [Required(ErrorMessage = "Leave type is required.")]
    [Display(Name = "Leave Type")]
    public int LeaveTypeId { get; set; }

    [Display(Name = "Leave Type")]
    public string? LeaveTypeName { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Display(Name = "Number of Days")]
    public int NumberOfDays { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
    public string Reason { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    [StringLength(500, ErrorMessage = "Manager comment cannot exceed 500 characters.")]
    [Display(Name = "Manager Comment")]
    public string? ManagerComment { get; set; }

    [Display(Name = "Applied Date")]
    public DateTime AppliedDate { get; set; } = DateTime.Now;

    [Display(Name = "Reviewed Date")]
    public DateTime? ReviewedDate { get; set; }

    [Display(Name = "Reviewed By")]
    public string? ReviewedBy { get; set; }
}
