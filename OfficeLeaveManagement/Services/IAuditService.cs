namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.Models;
using System.Collections.Generic;

public interface IAuditService
{
    void Log(string userId, string action, string description);
    List<AuditLog> GetAll();
    List<AuditLog> Search(string? userId, string? action, string? searchTerm);
}
