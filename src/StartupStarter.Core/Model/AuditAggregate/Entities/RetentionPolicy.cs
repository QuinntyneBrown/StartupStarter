using StartupStarter.Core.Model.AuditAggregate.Events;

namespace StartupStarter.Core.Model.AuditAggregate.Entities;

public class RetentionPolicy
{
    public string RetentionPolicyId { get; private set; }
    public string PolicyName { get; private set; }
    public int RetentionDays { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private RetentionPolicy()
    {
        RetentionPolicyId = string.Empty;
        PolicyName = string.Empty;
    }

    public RetentionPolicy(string retentionPolicyId, string policyName, int retentionDays)
    {
        if (string.IsNullOrWhiteSpace(retentionPolicyId))
            throw new ArgumentException("RetentionPolicy ID cannot be empty", nameof(retentionPolicyId));
        if (string.IsNullOrWhiteSpace(policyName))
            throw new ArgumentException("Policy name cannot be empty", nameof(policyName));
        if (retentionDays < 1)
            throw new ArgumentException("Retention days must be greater than 0", nameof(retentionDays));

        RetentionPolicyId = retentionPolicyId;
        PolicyName = policyName;
        RetentionDays = retentionDays;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Apply(int deletedRecordCount, DateTime oldestDateRetained)
    {
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AuditRetentionPolicyAppliedEvent
        {
            RetentionPolicyId = RetentionPolicyId,
            DeletedRecordCount = deletedRecordCount,
            OldestDateRetained = oldestDateRetained,
            Timestamp = UpdatedAt.Value
        });
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
