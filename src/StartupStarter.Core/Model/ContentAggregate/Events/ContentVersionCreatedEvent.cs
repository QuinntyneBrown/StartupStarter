namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentVersionCreatedEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string ChangeDescription { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
