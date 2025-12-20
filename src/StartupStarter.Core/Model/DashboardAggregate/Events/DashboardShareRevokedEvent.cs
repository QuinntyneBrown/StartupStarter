namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardShareRevokedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> RevokedUserIds { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
