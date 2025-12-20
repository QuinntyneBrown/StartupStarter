namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentUnpublishedEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UnpublishedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
