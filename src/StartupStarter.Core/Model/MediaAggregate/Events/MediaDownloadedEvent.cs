namespace StartupStarter.Core.Model.MediaAggregate.Events;

public class MediaDownloadedEvent : DomainEvent
{
    public string MediaId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DownloadedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
