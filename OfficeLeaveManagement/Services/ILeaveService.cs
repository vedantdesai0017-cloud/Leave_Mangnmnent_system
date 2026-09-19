namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.Models;
using System;
using System.Collections.Generic;

public interface ILeaveService
{
    // Leave Types
    List<LeaveType> GetAllLeaveTypes();
    List<LeaveType> GetActiveLeaveTypes();
    LeaveType? GetLeaveTypeById(int id);
    void AddLeaveType(LeaveType leaveType);
    void UpdateLeaveType(LeaveType leaveType);

    // Leave Balances
    List<LeaveBalance> GetEmployeeBalances(int employeeId);
    LeaveBalance? GetBalance(int employeeId, int leaveTypeId);
    void UpdateBalance(int employeeId, int leaveTypeId, int newTotal);
    void InitializeBalancesForEmployee(int employeeId);

    // Leave Applications
    List<LeaveApplication> GetAll();
    List<LeaveApplication> GetByEmployee(int employeeId);
    List<LeaveApplication> GetPendingByDepartment(int departmentId);
    List<LeaveApplication> GetByDepartment(int departmentId);
    LeaveApplication? GetById(int id);
    List<LeaveApplication> Search(int? employeeId, int? departmentId, int? leaveTypeId, LeaveStatus? status, DateTime? startDate, DateTime? endDate);

    // Core workflow
    int CalculateLeaveDays(DateTime startDate, DateTime endDate);
    bool HasOverlappingLeave(int employeeId, DateTime startDate, DateTime endDate, int? excludeApplicationId = null);
    (bool Success, string Message) ApplyLeave(LeaveApplication application);
    (bool Success, string Message) ApproveLeave(int applicationId, string reviewedBy);
    (bool Success, string Message) RejectLeave(int applicationId, string reviewedBy, string comment);
    (bool Success, string Message) CancelLeave(int applicationId, int employeeId);

    // Statistics
    int GetEmployeesOnLeaveToday();
    int GetEmployeesOnLeaveTodayByDepartment(int departmentId);
}
