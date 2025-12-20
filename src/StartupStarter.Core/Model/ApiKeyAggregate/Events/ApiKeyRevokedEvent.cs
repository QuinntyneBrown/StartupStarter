namespace StartupStarter.Core.Model.ApiKeyAggregate.Events;

public class ApiKeyRevokedEvent : DomainEvent
{
    public string ApiKeyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RevokedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
