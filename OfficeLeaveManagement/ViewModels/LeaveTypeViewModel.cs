using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.ViewModels;

public class LeaveTypeViewModel
{
    public int LeaveTypeId { get; set; }

    [Required(ErrorMessage = "Leave name is required")]
    [StringLength(100, ErrorMessage = "Leave name cannot exceed 100 characters")]
    [Display(Name = "Leave Type Name")]
    public string LeaveName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Annual allocation is required")]
    [Range(0, 365, ErrorMessage = "Annual allocation must be between 0 and 365 days")]
    [Display(Name = "Annual Allocation (Days)")]
    public int AnnualAllocation { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
