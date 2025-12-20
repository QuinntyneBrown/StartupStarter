namespace StartupStarter.Core.Model.WebhookAggregate.Events;

public class WebhookTriggeredEvent : DomainEvent
{
    public string WebhookId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public object PayloadSent { get; set; } = new();
    public int ResponseStatus { get; set; }
    public DateTime Timestamp { get; set; }
}
