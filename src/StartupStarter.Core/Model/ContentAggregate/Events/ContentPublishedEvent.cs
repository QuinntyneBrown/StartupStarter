namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentPublishedEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string PublishedBy { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public bool ScheduledPublish { get; set; }
    public DateTime Timestamp { get; set; }
}
