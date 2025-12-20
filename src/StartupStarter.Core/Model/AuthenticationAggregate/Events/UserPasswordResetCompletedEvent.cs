using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserPasswordResetCompletedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public ResetMethod ResetMethod { get; set; }
}
