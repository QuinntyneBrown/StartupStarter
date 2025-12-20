using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserActivatedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ActivatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public ActivationMethod ActivationMethod { get; set; }
}
