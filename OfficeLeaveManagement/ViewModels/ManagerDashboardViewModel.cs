using System.ComponentModel.DataAnnotations;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.ViewModels;

public class ManagerDashboardViewModel
{
    public string ManagerName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;

    public int PendingApprovals { get; set; }
    public int EmployeesOnLeave { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int TotalEmployees { get; set; }

    public List<LeaveApplication> PendingLeaveApplications { get; set; } = new();

    public List<string> DepartmentLabels { get; set; } = new();
    public List<int> DepartmentLeaveCounts { get; set; } = new();
}
