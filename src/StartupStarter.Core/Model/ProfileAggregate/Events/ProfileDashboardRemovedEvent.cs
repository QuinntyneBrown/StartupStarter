namespace StartupStarter.Core.Model.ProfileAggregate.Events;

public class ProfileDashboardRemovedEvent : DomainEvent
{
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DashboardId { get; set; } = string.Empty;
    public string RemovedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
