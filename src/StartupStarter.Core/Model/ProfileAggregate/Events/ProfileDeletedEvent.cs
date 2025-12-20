namespace StartupStarter.Core.Model.ProfileAggregate.Events;

public class ProfileDeletedEvent : DomainEvent
{
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public int DashboardCount { get; set; }
    public DateTime Timestamp { get; set; }
}
