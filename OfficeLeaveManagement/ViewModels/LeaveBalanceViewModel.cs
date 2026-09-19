namespace OfficeLeaveManagement.ViewModels;

public class LeaveBalanceViewModel
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public List<LeaveBalanceItem> Balances { get; set; } = new();
}

public class LeaveBalanceItem
{
    public int LeaveBalanceId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public int TotalLeaves { get; set; }
    public int UsedLeaves { get; set; }
    public int RemainingLeaves { get; set; }
    public int Year { get; set; }
}
