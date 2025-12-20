namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserCreatedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public List<string> InitialRoles { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public bool InvitationSent { get; set; }
}
