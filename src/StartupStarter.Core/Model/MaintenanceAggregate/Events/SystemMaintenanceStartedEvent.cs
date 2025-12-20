using StartupStarter.Core.Model.MaintenanceAggregate.Enums;

namespace StartupStarter.Core.Model.MaintenanceAggregate.Events;

public class SystemMaintenanceStartedEvent : DomainEvent
{
    public string MaintenanceId { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ActualStartTime { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public List<string> AffectedServices { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
