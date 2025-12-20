namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentUpdatedEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedFields { get; set; } = new();
    public int VersionNumber { get; set; }
    public DateTime Timestamp { get; set; }
}
