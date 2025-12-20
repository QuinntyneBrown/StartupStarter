namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardCreatedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string DashboardName { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string Template { get; set; } = string.Empty;
    public string LayoutType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
