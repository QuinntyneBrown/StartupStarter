namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserInvitationAcceptedEvent : DomainEvent
{
    public string InvitationId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
