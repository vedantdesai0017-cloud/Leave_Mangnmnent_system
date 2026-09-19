using System.ComponentModel.DataAnnotations;

namespace OfficeLeaveManagement.Models;

public class AuditLog
{
    public int AuditLogId { get; set; }

    [Display(Name = "User ID")]
    public string? UserId { get; set; }

    [Required(ErrorMessage = "Action is required.")]
    public string Action { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.Now;
}
