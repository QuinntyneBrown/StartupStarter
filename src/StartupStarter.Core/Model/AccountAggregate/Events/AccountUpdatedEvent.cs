namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountUpdatedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedFields { get; set; } = new();
    public Dictionary<string, object> PreviousValues { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
