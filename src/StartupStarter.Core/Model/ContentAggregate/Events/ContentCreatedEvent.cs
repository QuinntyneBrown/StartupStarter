using StartupStarter.Core.Model.ContentAggregate.Enums;

namespace StartupStarter.Core.Model.ContentAggregate.Events;

public class ContentCreatedEvent : DomainEvent
{
    public string ContentId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}
