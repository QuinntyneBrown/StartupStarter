namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardSharedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> SharedWithUserIds { get; set; } = new();
    public string PermissionLevel { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
