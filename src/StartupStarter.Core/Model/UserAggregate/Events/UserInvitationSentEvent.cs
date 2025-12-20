namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserInvitationSentEvent : DomainEvent
{
    public string InvitationId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string InvitedBy { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
    public DateTime Timestamp { get; set; }
}
