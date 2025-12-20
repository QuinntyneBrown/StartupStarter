namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountSubscriptionChangedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string PreviousTier { get; set; } = string.Empty;
    public string NewTier { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime Timestamp { get; set; }
}
