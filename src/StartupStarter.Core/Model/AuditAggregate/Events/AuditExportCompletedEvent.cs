using StartupStarter.Core.Model.AuditAggregate.Enums;

namespace StartupStarter.Core.Model.AuditAggregate.Events;

public class AuditExportCompletedEvent : DomainEvent
{
    public string ExportId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public FileFormat FileFormat { get; set; }
    public int RecordCount { get; set; }
    public string FileLocation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
