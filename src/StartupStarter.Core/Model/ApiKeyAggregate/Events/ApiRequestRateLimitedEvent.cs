namespace StartupStarter.Core.Model.ApiKeyAggregate.Events;

public class ApiRequestRateLimitedEvent : DomainEvent
{
    public string RequestId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKeyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RateLimitTier { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
