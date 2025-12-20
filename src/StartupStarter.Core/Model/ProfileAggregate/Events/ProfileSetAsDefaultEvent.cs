namespace StartupStarter.Core.Model.ProfileAggregate.Events;

public class ProfileSetAsDefaultEvent : DomainEvent
{
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? PreviousDefaultProfileId { get; set; }
    public DateTime Timestamp { get; set; }
}
