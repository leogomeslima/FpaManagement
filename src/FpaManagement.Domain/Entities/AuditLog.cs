using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId
    {
        get; private set;
    }
    public string? UserEmail
    {
        get; private set;
    }
    public string Action
    {
        get; private set;
    }
    public string EntityName
    {
        get; private set;
    }
    public string? EntityId
    {
        get; private set;
    }
    public string? OldValues
    {
        get; private set;
    }
    public string? NewValues
    {
        get; private set;
    }
    public string? Changes
    {
        get; private set;
    }
    public string? IpAddress
    {
        get; private set;
    }
    public string? UserAgent
    {
        get; private set;
    }
    public DateTime Timestamp
    {
        get; private set;
    }

    // Navigation property
    public virtual User? User
    {
        get; private set;
    }

    private AuditLog()
    {
    } // For EF Core

    public AuditLog(
        string action,
        string entityName,
        Guid? userId = null,
        string? userEmail = null,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? changes = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required", nameof(action));

        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException("Entity name is required", nameof(entityName));

        Action = action;
        EntityName = entityName;
        UserId = userId;
        UserEmail = userEmail;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        Changes = changes;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
    }
}
