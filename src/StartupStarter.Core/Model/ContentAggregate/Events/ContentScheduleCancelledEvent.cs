namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentScheduleCancelledEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CancelledBy { get; set; } = string.Empty;
    public DateTime OriginalScheduledDate { get; set; }
    public DateTime Timestamp { get; set; }
}
