namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserLockedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string LockedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; } = string.Empty;
    public TimeSpan? LockDuration { get; set; }
}
