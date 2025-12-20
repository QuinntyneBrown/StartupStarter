namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserPasswordResetRequestedEvent : DomainEvent
{
    public string Email { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ResetToken { get; set; } = string.Empty;
}
