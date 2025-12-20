# Content Management Models

## Core Aggregate

### ContentAggregate

Located in: `StartupStarter.Core\Model\ContentAggregate\`

#### Entities

**Content.cs** (Aggregate Root)
```csharp
public class Content
{
    public string ContentId { get; private set; }
    public string ContentType { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public string AuthorId { get; private set; }
    public string AccountId { get; private set; }
    public string ProfileId { get; private set; }
    public ContentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? UnpublishedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DeletionType? DeletionType { get; private set; }
    public DateTime? ScheduledPublishDate { get; private set; }
    public int CurrentVersion { get; private set; }

    private readonly List<ContentVersion> _versions = new();
    public IReadOnlyCollection<ContentVersion> Versions => _versions.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Update(Dictionary<string, object> updatedFields, string updatedBy);
    public void Publish(string publishedBy, DateTime? publishDate = null);
    public void Unpublish(string unpublishedBy, string reason);
    public void ChangeStatus(ContentStatus newStatus, string changedBy);
    public void Delete(string deletedBy, DeletionType deletionType);
    public void CreateVersion(string createdBy, string changeDescription);
    public void RestoreVersion(int versionNumber, string restoredBy);
    public void Schedule(DateTime publishDate, string scheduledBy);
    public void CancelSchedule(string cancelledBy);
}
```

**ContentVersion.cs**
```csharp
public class ContentVersion
{
    public string ContentVersionId { get; private set; }
    public string ContentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public string CreatedBy { get; private set; }
    public string ChangeDescription { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Content Content { get; private set; }
}
```

#### Enums

**ContentStatus.cs**
```csharp
public enum ContentStatus
{
    Draft,
    Review,
    Approved,
    Published,
    Unpublished,
    Archived,
    Deleted
}
```

**DeletionType.cs**
```csharp
public enum DeletionType
{
    SoftDelete,
    HardDelete
}
```

#### Domain Events

**ContentCreatedEvent.cs**
```csharp
public class ContentCreatedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string ContentType { get; set; }
    public string Title { get; set; }
    public string AuthorId { get; set; }
    public string AccountId { get; set; }
    public string ProfileId { get; set; }
    public ContentStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentUpdatedEvent.cs**
```csharp
public class ContentUpdatedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string ContentType { get; set; }
    public string AccountId { get; set; }
    public string UpdatedBy { get; set; }
    public Dictionary<string, object> UpdatedFields { get; set; }
    public int VersionNumber { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentDeletedEvent.cs**
```csharp
public class ContentDeletedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string ContentType { get; set; }
    public string AccountId { get; set; }
    public string DeletedBy { get; set; }
    public DeletionType DeletionType { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentPublishedEvent.cs**
```csharp
public class ContentPublishedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string ContentType { get; set; }
    public string AccountId { get; set; }
    public string PublishedBy { get; set; }
    public DateTime PublishDate { get; set; }
    public bool ScheduledPublish { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentUnpublishedEvent.cs**
```csharp
public class ContentUnpublishedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string ContentType { get; set; }
    public string AccountId { get; set; }
    public string UnpublishedBy { get; set; }
    public string Reason { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentStatusChangedEvent.cs**
```csharp
public class ContentStatusChangedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string ContentType { get; set; }
    public string AccountId { get; set; }
    public string PreviousStatus { get; set; }
    public string NewStatus { get; set; }
    public string ChangedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentVersionCreatedEvent.cs**
```csharp
public class ContentVersionCreatedEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string AccountId { get; set; }
    public int VersionNumber { get; set; }
    public string CreatedBy { get; set; }
    public string ChangeDescription { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentVersionRestoredEvent.cs**
```csharp
public class ContentVersionRestoredEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string AccountId { get; set; }
    public int RestoredVersionNumber { get; set; }
    public int CurrentVersionNumber { get; set; }
    public string RestoredBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentScheduledEvent.cs**
```csharp
public class ContentScheduledEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string AccountId { get; set; }
    public DateTime ScheduledPublishDate { get; set; }
    public string ScheduledBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ContentScheduleCancelledEvent.cs**
```csharp
public class ContentScheduleCancelledEvent : DomainEvent
{
    public string ContentId { get; set; }
    public string AccountId { get; set; }
    public string CancelledBy { get; set; }
    public DateTime OriginalScheduledDate { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\Content\Commands\`

**CreateContentCommand.cs**
```csharp
public class CreateContentCommand : IRequest<ContentDto>
{
    public string ContentType { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string AuthorId { get; set; }
    public string AccountId { get; set; }
    public string ProfileId { get; set; }
}
```

**UpdateContentCommand.cs**
```csharp
public class UpdateContentCommand : IRequest<ContentDto>
{
    public string ContentId { get; set; }
    public Dictionary<string, object> UpdatedFields { get; set; }
    public string UpdatedBy { get; set; }
}
```

**PublishContentCommand.cs**
```csharp
public class PublishContentCommand : IRequest<bool>
{
    public string ContentId { get; set; }
    public string PublishedBy { get; set; }
    public DateTime? PublishDate { get; set; }
}
```

**DeleteContentCommand.cs**
```csharp
public class DeleteContentCommand : IRequest<bool>
{
    public string ContentId { get; set; }
    public string DeletedBy { get; set; }
    public DeletionType DeletionType { get; set; }
}
```

**ScheduleContentCommand.cs**
```csharp
public class ScheduleContentCommand : IRequest<bool>
{
    public string ContentId { get; set; }
    public DateTime ScheduledPublishDate { get; set; }
    public string ScheduledBy { get; set; }
}
```
