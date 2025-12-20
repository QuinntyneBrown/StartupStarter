namespace StartupStarter.Core.Model.AuditAggregate.Events;

public class AuditLogCreatedEvent : DomainEvent
{
    public string AuditId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public object BeforeState { get; set; } = new();
    public object AfterState { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
