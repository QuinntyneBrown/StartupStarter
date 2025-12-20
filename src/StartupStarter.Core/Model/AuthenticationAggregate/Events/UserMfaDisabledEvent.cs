namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserMfaDisabledEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DisabledBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; } = string.Empty;
}
