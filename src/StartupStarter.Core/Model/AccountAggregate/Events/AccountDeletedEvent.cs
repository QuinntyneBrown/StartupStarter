using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountDeletedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string OwnerUserId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int ProfileCount { get; set; }
    public int ContentCount { get; set; }
    public DateTime Timestamp { get; set; }
    public DeletionType DeletionType { get; set; }
}
