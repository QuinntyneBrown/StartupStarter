namespace StartupStarter.Core.Model.UserAggregate.Events;

public class UserUpdatedEvent : DomainEvent
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedFields { get; set; } = new();
    public Dictionary<string, object> PreviousValues { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
