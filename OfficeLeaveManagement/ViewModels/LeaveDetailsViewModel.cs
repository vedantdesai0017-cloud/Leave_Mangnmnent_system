using System.ComponentModel.DataAnnotations;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.ViewModels;

public class LeaveDetailsViewModel
{
    public LeaveApplication Application { get; set; } = null!;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string LeaveTypeName { get; set; } = string.Empty;
    public int LeaveBalanceRemaining { get; set; }
    public bool CanCancel { get; set; }
    public bool CanApproveReject { get; set; }
}
