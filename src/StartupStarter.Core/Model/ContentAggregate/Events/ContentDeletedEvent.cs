using StartupStarter.Core.Model.ContentAggregate.Enums;

namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentDeletedEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DeletionType DeletionType { get; set; }
    public DateTime Timestamp { get; set; }
}
