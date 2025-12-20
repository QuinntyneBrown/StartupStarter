namespace StartupStarter.Core.Model.WebhookAggregate.Events;

public class WebhookDeletedEvent : DomainEvent
{
    public string WebhookId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
