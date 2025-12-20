namespace StartupStarter.Core.Model.WebhookAggregate.Events;

public class WebhookRegisteredEvent : DomainEvent
{
    public string WebhookId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public string AccountId { get; set; } = string.Empty;
    public string RegisteredBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
