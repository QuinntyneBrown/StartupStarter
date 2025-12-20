using StartupStarter.Core.Model.MediaAggregate.Enums;

namespace StartupStarter.Core.Model.MediaAggregate.Events;

public class MediaDeletedEvent : DomainEvent
{
    public string MediaId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DeletionType DeletionType { get; set; }
    public DateTime Timestamp { get; set; }
}
