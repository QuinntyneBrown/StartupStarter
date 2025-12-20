using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserMfaEnabledEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public MfaMethod MfaMethod { get; set; }
    public string EnabledBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
