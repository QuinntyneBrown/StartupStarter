using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Events;

public class UserLoginSucceededEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public LoginMethod LoginMethod { get; set; }
    public string SessionId { get; set; } = string.Empty;
}
