namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class AuditService : IAuditService
{
    private readonly List<AuditLog> _logs = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public AuditService()
    {
        Log("EMP001", "LOGIN", "Admin user logged in.");
        Log("EMP002", "LOGIN", "Manager user logged in.");
        Log("EMP003", "LOGIN", "Employee user logged in.");
        Log("EMP004", "LOGIN", "Rahul Sharma logged in.");
        Log("EMP001", "LOGIN", "Admin user logged in.");
    }

    public void Log(string userId, string action, string description)
    {
        lock (_lock)
        {
            var log = new AuditLog
            {
                AuditLogId = _nextId++,
                UserId = userId,
                Action = action,
                Description = description,
                Timestamp = DateTime.Now
            };
            _logs.Add(log);
        }
    }

    public List<AuditLog> GetAll()
    {
        lock (_lock)
        {
            return _logs.OrderByDescending(l => l.Timestamp).ToList();
        }
    }

    public List<AuditLog> Search(string? userId, string? action, string? searchTerm)
    {
        lock (_lock)
        {
            var query = _logs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(l => l.UserId != null && l.UserId.Contains(userId, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(action))
                query = query.Where(l => l.Action.Contains(action, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(l => l.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return query.OrderByDescending(l => l.Timestamp).ToList();
        }
    }
}
