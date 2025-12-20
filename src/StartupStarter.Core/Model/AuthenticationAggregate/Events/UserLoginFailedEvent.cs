using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserLoginFailedEvent : DomainEvent
{
    public string Email { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public FailureReason FailureReason { get; set; }
    public int AttemptCount { get; set; }
}
