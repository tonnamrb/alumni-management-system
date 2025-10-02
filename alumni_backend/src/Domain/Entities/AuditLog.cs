using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Audit logging for all moderation actions and important system events
/// Supports FR-023: Audit Logging
/// </summary>
public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public AuditAction Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // Navigation Properties
    public User? User { get; set; }

    // Domain Methods
    public static AuditLog Create(
        AuditAction action, 
        string entityType, 
        int entityId, 
        int? userId = null,
        string? oldValues = null, 
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }

    public bool IsUserAction()
    {
        return UserId.HasValue;
    }

    public bool IsSystemAction()
    {
        return !UserId.HasValue;
    }

    public string GetActionDescription()
    {
        var actorDescription = IsUserAction() ? $"User {UserId}" : "System";
        return $"{actorDescription} performed {Action} on {EntityType} (ID: {EntityId})";
    }
}