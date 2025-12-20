namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserSessionExpiredEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan SessionDuration { get; set; }
}
