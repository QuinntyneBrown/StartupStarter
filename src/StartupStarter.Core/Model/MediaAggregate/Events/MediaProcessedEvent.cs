namespace StartupStarter.Core.Model.MediaAggregate.Events;

public class MediaProcessedEvent : DomainEvent
{
    public string MediaId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ProcessingType { get; set; } = string.Empty;
    public List<string> OutputFormats { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
}
