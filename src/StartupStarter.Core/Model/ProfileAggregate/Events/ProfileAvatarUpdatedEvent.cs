namespace StartupStarter.Core.Model.ProfileAggregate.Events;

public class ProfileAvatarUpdatedEvent : DomainEvent
{
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string? PreviousAvatarUrl { get; set; }
    public string NewAvatarUrl { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
