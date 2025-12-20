namespace StartupStarter.Core.Model.WebhookAggregate.Events;

public class WebhookFailedEvent : DomainEvent
{
    public string WebhookId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string FailureReason { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public DateTime Timestamp { get; set; }
}
