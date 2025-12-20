namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserAccountChangedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string PreviousAccountId { get; set; } = string.Empty;
    public string NewAccountId { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
