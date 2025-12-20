namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardSetAsDefaultEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string SetBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
