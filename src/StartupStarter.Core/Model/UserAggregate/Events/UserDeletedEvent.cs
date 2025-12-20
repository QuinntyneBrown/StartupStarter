using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserDeletedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DeletionType DeletionType { get; set; }
    public string Reason { get; set; } = string.Empty;
}
