namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentScheduledEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public DateTime ScheduledPublishDate { get; set; }
    public string ScheduledBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
