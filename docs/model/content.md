# Content Domain Models

This document defines the C# models needed to implement the content management events following Clean Architecture principles with MediatR.

## Domain Entities

### Content (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class Content : AggregateRoot
    {
        public Guid ContentId { get; private set; }
        public string ContentType { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public Guid AuthorId { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid? ProfileId { get; private set; }
        public ContentStatus Status { get; private set; }
        public int CurrentVersionNumber { get; private set; }
        public DateTime? PublishDate { get; private set; }
        public DateTime? ScheduledPublishDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        // Navigation properties
        public virtual User Author { get; private set; }
        public virtual Account Account { get; private set; }
        public virtual Profile? Profile { get; private set; }
        public virtual ICollection<ContentVersion> Versions { get; private set; } = new List<ContentVersion>();
        public virtual ICollection<ContentMetadata> Metadata { get; private set; } = new List<ContentMetadata>();

        private Content() { } // EF Core

        public static Content Create(
            string contentType,
            string title,
            string body,
            Guid authorId,
            Guid accountId,
            Guid? profileId = null)
        {
            var content = new Content
            {
                ContentId = Guid.NewGuid(),
                ContentType = contentType,
                Title = title,
                Body = body,
                AuthorId = authorId,
                AccountId = accountId,
                ProfileId = profileId,
                Status = ContentStatus.Draft,
                CurrentVersionNumber = 1,
                CreatedAt = DateTime.UtcNow
            };

            content.AddDomainEvent(new ContentCreatedDomainEvent(
                content.ContentId,
                content.ContentType,
                content.Title,
                content.AuthorId,
                content.AccountId,
                content.ProfileId,
                content.Status));

            return content;
        }

        public void Update(string title, string body, Dictionary<string, object> updatedFields, Guid updatedBy)
        {
            Title = title;
            Body = body;
            UpdatedAt = DateTime.UtcNow;
            CurrentVersionNumber++;

            AddDomainEvent(new ContentUpdatedDomainEvent(
                ContentId,
                ContentType,
                AccountId,
                updatedBy,
                updatedFields,
                CurrentVersionNumber));
        }

        public void Publish(Guid publishedBy, bool isScheduled = false)
        {
            Status = ContentStatus.Published;
            PublishDate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ContentPublishedDomainEvent(
                ContentId,
                ContentType,
                AccountId,
                publishedBy,
                PublishDate.Value,
                isScheduled));
        }

        public void Unpublish(Guid unpublishedBy, string reason)
        {
            Status = ContentStatus.Draft;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ContentUnpublishedDomainEvent(
                ContentId,
                ContentType,
                AccountId,
                unpublishedBy,
                reason));
        }

        public void ChangeStatus(ContentStatus newStatus, Guid changedBy)
        {
            var previousStatus = Status;
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ContentStatusChangedDomainEvent(
                ContentId,
                ContentType,
                AccountId,
                previousStatus.ToString(),
                newStatus.ToString(),
                changedBy));
        }

        public void CreateVersion(string changeDescription, Guid createdBy)
        {
            CurrentVersionNumber++;

            AddDomainEvent(new ContentVersionCreatedDomainEvent(
                ContentId,
                AccountId,
                CurrentVersionNumber,
                createdBy,
                changeDescription));
        }

        public void RestoreVersion(int versionNumber, Guid restoredBy)
        {
            var previousVersion = CurrentVersionNumber;
            CurrentVersionNumber++;

            AddDomainEvent(new ContentVersionRestoredDomainEvent(
                ContentId,
                AccountId,
                versionNumber,
                CurrentVersionNumber,
                restoredBy));
        }

        public void Schedule(DateTime scheduledDate, Guid scheduledBy)
        {
            ScheduledPublishDate = scheduledDate;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ContentScheduledDomainEvent(
                ContentId,
                AccountId,
                scheduledDate,
                scheduledBy));
        }

        public void CancelSchedule(Guid cancelledBy)
        {
            var originalDate = ScheduledPublishDate;
            ScheduledPublishDate = null;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ContentScheduleCancelledDomainEvent(
                ContentId,
                AccountId,
                cancelledBy,
                originalDate!.Value));
        }

        public void Delete(Guid deletedBy, DeletionType deletionType)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;

            AddDomainEvent(new ContentDeletedDomainEvent(
                ContentId,
                ContentType,
                AccountId,
                deletedBy,
                deletionType));
        }
    }
}
```

### ContentVersion (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class ContentVersion
    {
        public Guid VersionId { get; private set; }
        public Guid ContentId { get; private set; }
        public int VersionNumber { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public string ChangeDescription { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation
        public virtual Content Content { get; private set; }
        public virtual User Creator { get; private set; }

        private ContentVersion() { } // EF Core

        public static ContentVersion Create(
            Guid contentId,
            int versionNumber,
            string title,
            string body,
            string changeDescription,
            Guid createdBy)
        {
            return new ContentVersion
            {
                VersionId = Guid.NewGuid(),
                ContentId = contentId,
                VersionNumber = versionNumber,
                Title = title,
                Body = body,
                ChangeDescription = changeDescription,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
```

### ContentMetadata (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class ContentMetadata
    {
        public Guid MetadataId { get; private set; }
        public Guid ContentId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string DataType { get; private set; }

        // Navigation
        public virtual Content Content { get; private set; }

        private ContentMetadata() { } // EF Core

        public static ContentMetadata Create(Guid contentId, string key, string value, string dataType)
        {
            return new ContentMetadata
            {
                MetadataId = Guid.NewGuid(),
                ContentId = contentId,
                Key = key,
                Value = value,
                DataType = dataType
            };
        }

        public void UpdateValue(string newValue)
        {
            Value = newValue;
        }
    }
}
```

## Enumerations

### ContentStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ContentStatus
    {
        Draft = 0,
        Review = 1,
        Published = 2,
        Archived = 3,
        Scheduled = 4
    }
}
```

### DeletionType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum DeletionType
    {
        SoftDelete = 0,
        HardDelete = 1
    }
}
```

## Domain Events

### ContentCreatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentCreatedDomainEvent(
        Guid ContentId,
        string ContentType,
        string Title,
        Guid AuthorId,
        Guid AccountId,
        Guid? ProfileId,
        ContentStatus Status) : DomainEvent;
}
```

### ContentUpdatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentUpdatedDomainEvent(
        Guid ContentId,
        string ContentType,
        Guid AccountId,
        Guid UpdatedBy,
        Dictionary<string, object> UpdatedFields,
        int VersionNumber) : DomainEvent;
}
```

### ContentDeletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentDeletedDomainEvent(
        Guid ContentId,
        string ContentType,
        Guid AccountId,
        Guid DeletedBy,
        DeletionType DeletionType) : DomainEvent;
}
```

### ContentPublishedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentPublishedDomainEvent(
        Guid ContentId,
        string ContentType,
        Guid AccountId,
        Guid PublishedBy,
        DateTime PublishDate,
        bool ScheduledPublish) : DomainEvent;
}
```

### ContentUnpublishedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentUnpublishedDomainEvent(
        Guid ContentId,
        string ContentType,
        Guid AccountId,
        Guid UnpublishedBy,
        string Reason) : DomainEvent;
}
```

### ContentStatusChangedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentStatusChangedDomainEvent(
        Guid ContentId,
        string ContentType,
        Guid AccountId,
        string PreviousStatus,
        string NewStatus,
        Guid ChangedBy) : DomainEvent;
}
```

### ContentVersionCreatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentVersionCreatedDomainEvent(
        Guid ContentId,
        Guid AccountId,
        int VersionNumber,
        Guid CreatedBy,
        string ChangeDescription) : DomainEvent;
}
```

### ContentVersionRestoredDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentVersionRestoredDomainEvent(
        Guid ContentId,
        Guid AccountId,
        int RestoredVersionNumber,
        int CurrentVersionNumber,
        Guid RestoredBy) : DomainEvent;
}
```

### ContentScheduledDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentScheduledDomainEvent(
        Guid ContentId,
        Guid AccountId,
        DateTime ScheduledPublishDate,
        Guid ScheduledBy) : DomainEvent;
}
```

### ContentScheduleCancelledDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ContentScheduleCancelledDomainEvent(
        Guid ContentId,
        Guid AccountId,
        Guid CancelledBy,
        DateTime OriginalScheduledDate) : DomainEvent;
}
```

## MediatR Commands

### CreateContentCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record CreateContentCommand(
        string ContentType,
        string Title,
        string Body,
        Guid AuthorId,
        Guid AccountId,
        Guid? ProfileId = null) : IRequest<CreateContentResponse>;

    public record CreateContentResponse(
        Guid ContentId,
        ContentStatus Status,
        DateTime CreatedAt);
}
```

### UpdateContentCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record UpdateContentCommand(
        Guid ContentId,
        string Title,
        string Body,
        Dictionary<string, object> UpdatedFields,
        Guid UpdatedBy) : IRequest<UpdateContentResponse>;

    public record UpdateContentResponse(
        Guid ContentId,
        int VersionNumber,
        DateTime UpdatedAt);
}
```

### DeleteContentCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record DeleteContentCommand(
        Guid ContentId,
        Guid DeletedBy,
        DeletionType DeletionType = DeletionType.SoftDelete) : IRequest<Unit>;
}
```

### PublishContentCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record PublishContentCommand(
        Guid ContentId,
        Guid PublishedBy,
        bool IsScheduled = false) : IRequest<PublishContentResponse>;

    public record PublishContentResponse(
        Guid ContentId,
        DateTime PublishDate);
}
```

### UnpublishContentCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record UnpublishContentCommand(
        Guid ContentId,
        Guid UnpublishedBy,
        string Reason) : IRequest<Unit>;
}
```

### ChangeContentStatusCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record ChangeContentStatusCommand(
        Guid ContentId,
        ContentStatus NewStatus,
        Guid ChangedBy) : IRequest<Unit>;
}
```

### CreateContentVersionCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record CreateContentVersionCommand(
        Guid ContentId,
        string ChangeDescription,
        Guid CreatedBy) : IRequest<CreateContentVersionResponse>;

    public record CreateContentVersionResponse(
        Guid VersionId,
        int VersionNumber);
}
```

### RestoreContentVersionCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record RestoreContentVersionCommand(
        Guid ContentId,
        int VersionNumber,
        Guid RestoredBy) : IRequest<Unit>;
}
```

### ScheduleContentCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record ScheduleContentCommand(
        Guid ContentId,
        DateTime ScheduledPublishDate,
        Guid ScheduledBy) : IRequest<Unit>;
}
```

### CancelContentScheduleCommand

```csharp
namespace StartupStarter.Application.Content.Commands
{
    public record CancelContentScheduleCommand(
        Guid ContentId,
        Guid CancelledBy) : IRequest<Unit>;
}
```

## MediatR Queries

### GetContentByIdQuery

```csharp
namespace StartupStarter.Application.Content.Queries
{
    public record GetContentByIdQuery(Guid ContentId) : IRequest<ContentDto?>;
}
```

### GetContentsByAccountQuery

```csharp
namespace StartupStarter.Application.Content.Queries
{
    public record GetContentsByAccountQuery(
        Guid AccountId,
        ContentStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<ContentDto>>;
}
```

### GetContentVersionsQuery

```csharp
namespace StartupStarter.Application.Content.Queries
{
    public record GetContentVersionsQuery(Guid ContentId) : IRequest<List<ContentVersionDto>>;
}
```

## DTOs

### ContentDto

```csharp
namespace StartupStarter.Application.Content.DTOs
{
    public record ContentDto(
        Guid ContentId,
        string ContentType,
        string Title,
        string Body,
        Guid AuthorId,
        string AuthorName,
        Guid AccountId,
        Guid? ProfileId,
        ContentStatus Status,
        int CurrentVersionNumber,
        DateTime? PublishDate,
        DateTime? ScheduledPublishDate,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
```

### ContentVersionDto

```csharp
namespace StartupStarter.Application.Content.DTOs
{
    public record ContentVersionDto(
        Guid VersionId,
        int VersionNumber,
        string Title,
        string ChangeDescription,
        string CreatedByName,
        DateTime CreatedAt);
}
```

## Base Classes

### AggregateRoot

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
```

### DomainEvent

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
```

## Repository Interface

### IContentRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IContentRepository
    {
        Task<Content?> GetByIdAsync(Guid contentId, CancellationToken cancellationToken = default);
        Task<List<Content>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<PaginatedList<Content>> GetPagedByAccountIdAsync(
            Guid accountId,
            ContentStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<ContentVersion?> GetVersionAsync(Guid contentId, int versionNumber, CancellationToken cancellationToken = default);
        Task<List<ContentVersion>> GetVersionsAsync(Guid contentId, CancellationToken cancellationToken = default);
        Task AddAsync(Content content, CancellationToken cancellationToken = default);
        Task UpdateAsync(Content content, CancellationToken cancellationToken = default);
        Task DeleteAsync(Content content, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```
