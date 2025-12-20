namespace StartupStarter.Core.Model.MediaAggregate.Events;

public class MediaTaggedEvent : DomainEvent
{
    public string MediaId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string TaggedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
