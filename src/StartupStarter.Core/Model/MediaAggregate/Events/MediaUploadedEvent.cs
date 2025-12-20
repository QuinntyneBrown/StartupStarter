namespace StartupStarter.Core.Model.MediaAggregate.Events;

public class MediaUploadedEvent : DomainEvent
{
    public string MediaId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
