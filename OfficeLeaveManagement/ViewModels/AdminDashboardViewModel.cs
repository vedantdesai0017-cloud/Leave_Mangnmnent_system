using System.ComponentModel.DataAnnotations;
using OfficeLeaveManagement.Models;

namespace OfficeLeaveManagement.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int EmployeesOnLeave { get; set; }

    public List<string> StatusLabels { get; set; } = new();
    public List<int> StatusCounts { get; set; } = new();

    public List<string> DepartmentLabels { get; set; } = new();
    public List<int> DepartmentLeaveCounts { get; set; } = new();

    public List<string> MonthlyLabels { get; set; } = new();
    public List<int> MonthlyLeaveCounts { get; set; } = new();

    public List<LeaveApplication> RecentApplications { get; set; } = new();
}
