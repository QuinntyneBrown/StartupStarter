namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardMovedToProfileEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string PreviousProfileId { get; set; } = string.Empty;
    public string NewProfileId { get; set; } = string.Empty;
    public string MovedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
