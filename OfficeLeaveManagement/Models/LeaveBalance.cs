using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.Models;

public class LeaveBalance
{
    public int LeaveBalanceId { get; set; }

    [Required]
    [Display(Name = "Employee ID")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Leave Type ID")]
    public int LeaveTypeId { get; set; }

    [Display(Name = "Leave Type")]
    public string? LeaveTypeName { get; set; }

    [Display(Name = "Total Leaves")]
    public int TotalLeaves { get; set; }

    [Display(Name = "Used Leaves")]
    public int UsedLeaves { get; set; }

    [Display(Name = "Remaining Leaves")]
    public int RemainingLeaves => TotalLeaves - UsedLeaves;

    [Display(Name = "Year")]
    public int Year { get; set; } = DateTime.Today.Year;
}
