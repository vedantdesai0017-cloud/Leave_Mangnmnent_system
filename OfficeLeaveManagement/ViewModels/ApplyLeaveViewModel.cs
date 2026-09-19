using System.ComponentModel.DataAnnotations;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.ViewModels;

public class ApplyLeaveViewModel
{
    [Required(ErrorMessage = "Please select a leave type")]
    [Display(Name = "Leave Type")]
    public int LeaveTypeId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "End date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Display(Name = "Number of Days")]
    public int NumberOfDays { get; set; }

    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason")]
    public string Reason { get; set; } = string.Empty;

    public List<LeaveType>? AvailableLeaveTypes { get; set; }

    [Display(Name = "Available Balance")]
    public int? AvailableBalance { get; set; }
}
