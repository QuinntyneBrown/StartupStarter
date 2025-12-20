namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountReactivatedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string ReactivatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
