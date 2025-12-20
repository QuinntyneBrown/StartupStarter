namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentVersionRestoredEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public int RestoredVersionNumber { get; set; }
    public int CurrentVersionNumber { get; set; }
    public string RestoredBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
