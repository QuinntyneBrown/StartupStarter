namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountProfileAddedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
