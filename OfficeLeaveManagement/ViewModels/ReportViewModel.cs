namespace OfficeLeaveManagement.ViewModels;

public class ReportViewModel
{
    public List<string> MonthlyLabels { get; set; } = new();
    public List<int> MonthlyLeaveCounts { get; set; } = new();

    public List<string> DepartmentLabels { get; set; } = new();
    public List<int> DepartmentLeaveCounts { get; set; } = new();

    public List<string> StatusLabels { get; set; } = new();
    public List<int> StatusCounts { get; set; } = new();

    public List<EmployeeLeaveStatItem> EmployeeLeaveStats { get; set; } = new();
}

public class EmployeeLeaveStatItem
{
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int TotalApplied { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
}
