namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserUnlockedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UnlockedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
