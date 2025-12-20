namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardLayoutChangedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string PreviousLayout { get; set; } = string.Empty;
    public string NewLayout { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
