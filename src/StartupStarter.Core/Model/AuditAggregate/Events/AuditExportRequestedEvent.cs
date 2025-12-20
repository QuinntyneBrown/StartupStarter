namespace StartupStarter.Core.Model.AuditAggregate.Events;

public class AuditExportRequestedEvent : DomainEvent
{
    public string ExportId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public object DateRange { get; set; } = new();
    public Dictionary<string, object> Filters { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
