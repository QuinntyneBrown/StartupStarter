namespace StartupStarter.Core.Model.ApiKeyAggregate.Events;

public class ApiKeyCreatedEvent : DomainEvent
{
    public string ApiKeyId { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
    public DateTime Timestamp { get; set; }
}
