namespace StartupStarter.Core.Model.AuditAggregate.Events;

public class AuditRetentionPolicyAppliedEvent : DomainEvent
{
    public string RetentionPolicyId { get; set; } = string.Empty;
    public int DeletedRecordCount { get; set; }
    public DateTime OldestDateRetained { get; set; }
    public DateTime Timestamp { get; set; }
}
