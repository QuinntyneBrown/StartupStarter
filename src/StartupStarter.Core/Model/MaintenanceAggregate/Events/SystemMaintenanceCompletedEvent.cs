using StartupStarter.Core.Model.MaintenanceAggregate.Enums;

namespace StartupStarter.Core.Model.MaintenanceAggregate.Events;

public class SystemMaintenanceCompletedEvent : DomainEvent
{
    public string MaintenanceId { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ActualStartTime { get; set; }
    public DateTime CompletedTime { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public TimeSpan ActualDuration { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime Timestamp { get; set; }
}
