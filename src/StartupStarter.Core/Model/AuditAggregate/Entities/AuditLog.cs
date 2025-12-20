using System.Text.Json;
using StartupStarter.Core.Model.AuditAggregate.Events;

namespace StartupStarter.Core.Model.AuditAggregate.Entities;

public class AuditLog
{
    public string AuditId { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string AccountId { get; private set; }
    public string Action { get; private set; }
    public string PerformedBy { get; private set; }
    public string IPAddress { get; private set; }
    public string BeforeStateJson { get; private set; }
    public string AfterStateJson { get; private set; }
    public DateTime Timestamp { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private AuditLog()
    {
        AuditId = string.Empty;
        EntityType = string.Empty;
        EntityId = string.Empty;
        AccountId = string.Empty;
        Action = string.Empty;
        PerformedBy = string.Empty;
        IPAddress = string.Empty;
        BeforeStateJson = string.Empty;
        AfterStateJson = string.Empty;
    }

    public static AuditLog Create(string entityType, string entityId, string accountId, string action,
        string performedBy, string ipAddress, object beforeState, object afterState)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type cannot be empty", nameof(entityType));
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("Entity ID cannot be empty", nameof(entityId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));
        if (string.IsNullOrWhiteSpace(performedBy))
            throw new ArgumentException("PerformedBy cannot be empty", nameof(performedBy));

        var auditLog = new AuditLog
        {
            AuditId = Guid.NewGuid().ToString(),
            EntityType = entityType,
            EntityId = entityId,
            AccountId = accountId,
            Action = action,
            PerformedBy = performedBy,
            IPAddress = ipAddress ?? string.Empty,
            BeforeStateJson = beforeState != null ? JsonSerializer.Serialize(beforeState) : string.Empty,
            AfterStateJson = afterState != null ? JsonSerializer.Serialize(afterState) : string.Empty,
            Timestamp = DateTime.UtcNow
        };

        auditLog.AddDomainEvent(new AuditLogCreatedEvent
        {
            AuditId = auditLog.AuditId,
            EntityType = entityType,
            EntityId = entityId,
            AccountId = accountId,
            Action = action,
            PerformedBy = performedBy,
            IPAddress = ipAddress ?? string.Empty,
            BeforeState = beforeState,
            AfterState = afterState,
            Timestamp = auditLog.Timestamp
        });

        return auditLog;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
