namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardClonedEvent : DomainEvent
{
    public string OriginalDashboardId { get; set; } = string.Empty;
    public string NewDashboardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ClonedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
