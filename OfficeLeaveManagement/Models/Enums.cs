namespace OfficeLeaveManagement.Models;

public enum LeaveStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public enum UserRole
{
    Admin = 0,
    Manager = 1,
    Employee = 2
}
