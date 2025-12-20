using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountCreatedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
    public string SubscriptionTier { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
