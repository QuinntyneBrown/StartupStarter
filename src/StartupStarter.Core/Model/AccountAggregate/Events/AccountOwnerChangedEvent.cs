namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountOwnerChangedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string PreviousOwnerUserId { get; set; } = string.Empty;
    public string NewOwnerUserId { get; set; } = string.Empty;
    public string TransferredBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
