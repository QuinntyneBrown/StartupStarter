namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardDeletedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string DashboardName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
