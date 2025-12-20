using StartupStarter.Core.Model.ContentAggregate.Enums;
using StartupStarter.Core.Model.ContentAggregate.Events;

namespace StartupStarter.Core.Model.ContentAggregate.Entities;

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

    // EF Core constructor
    private Content()
    {
        ContentId = string.Empty;
        ContentType = string.Empty;
        Title = string.Empty;
        Body = string.Empty;
        AuthorId = string.Empty;
        AccountId = string.Empty;
        ProfileId = string.Empty;
    }

    public Content(string contentId, string contentType, string title, string body,
        string authorId, string accountId, string profileId)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(authorId))
            throw new ArgumentException("Author ID cannot be empty", nameof(authorId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));

        ContentId = contentId;
        ContentType = contentType ?? string.Empty;
        Title = title;
        Body = body ?? string.Empty;
        AuthorId = authorId;
        AccountId = accountId;
        ProfileId = profileId;
        Status = ContentStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        CurrentVersion = 1;

        AddDomainEvent(new ContentCreatedEvent
        {
            ContentId = ContentId,
            ContentType = ContentType,
            Title = Title,
            AuthorId = AuthorId,
            AccountId = AccountId,
            ProfileId = ProfileId,
            Status = Status,
            Timestamp = CreatedAt
        });
    }

    public void Update(Dictionary<string, object> updatedFields, string updatedBy)
    {
        if (updatedFields == null || !updatedFields.Any())
            throw new ArgumentException("Updated fields cannot be empty", nameof(updatedFields));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be empty", nameof(updatedBy));

        if (updatedFields.ContainsKey("Title"))
            Title = updatedFields["Title"].ToString() ?? Title;

        if (updatedFields.ContainsKey("Body"))
            Body = updatedFields["Body"].ToString() ?? Body;

        UpdatedAt = DateTime.UtcNow;
        CurrentVersion++;

        AddDomainEvent(new ContentUpdatedEvent
        {
            ContentId = ContentId,
            ContentType = ContentType,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            VersionNumber = CurrentVersion,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Publish(string publishedBy, DateTime? publishDate = null)
    {
        if (string.IsNullOrWhiteSpace(publishedBy))
            throw new ArgumentException("PublishedBy cannot be empty", nameof(publishedBy));

        var actualPublishDate = publishDate ?? DateTime.UtcNow;
        var isScheduled = publishDate.HasValue && publishDate.Value > DateTime.UtcNow;

        if (!isScheduled)
        {
            Status = ContentStatus.Published;
            PublishedAt = actualPublishDate;
            UpdatedAt = DateTime.UtcNow;
        }

        AddDomainEvent(new ContentPublishedEvent
        {
            ContentId = ContentId,
            ContentType = ContentType,
            AccountId = AccountId,
            PublishedBy = publishedBy,
            PublishDate = actualPublishDate,
            ScheduledPublish = isScheduled,
            Timestamp = DateTime.UtcNow
        });
    }

    public void Unpublish(string unpublishedBy, string reason)
    {
        if (string.IsNullOrWhiteSpace(unpublishedBy))
            throw new ArgumentException("UnpublishedBy cannot be empty", nameof(unpublishedBy));

        Status = ContentStatus.Unpublished;
        UnpublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContentUnpublishedEvent
        {
            ContentId = ContentId,
            ContentType = ContentType,
            AccountId = AccountId,
            UnpublishedBy = unpublishedBy,
            Reason = reason ?? string.Empty,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ChangeStatus(ContentStatus newStatus, string changedBy)
    {
        if (string.IsNullOrWhiteSpace(changedBy))
            throw new ArgumentException("ChangedBy cannot be empty", nameof(changedBy));

        var previousStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContentStatusChangedEvent
        {
            ContentId = ContentId,
            ContentType = ContentType,
            AccountId = AccountId,
            PreviousStatus = previousStatus.ToString(),
            NewStatus = newStatus.ToString(),
            ChangedBy = changedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy, DeletionType deletionType)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy cannot be empty", nameof(deletedBy));

        Status = ContentStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletionType = deletionType;

        AddDomainEvent(new ContentDeletedEvent
        {
            ContentId = ContentId,
            ContentType = ContentType,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            DeletionType = deletionType,
            Timestamp = UpdatedAt.Value
        });
    }

    public void CreateVersion(string createdBy, string changeDescription)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be empty", nameof(createdBy));

        CurrentVersion++;

        AddDomainEvent(new ContentVersionCreatedEvent
        {
            ContentId = ContentId,
            AccountId = AccountId,
            VersionNumber = CurrentVersion,
            CreatedBy = createdBy,
            ChangeDescription = changeDescription ?? string.Empty,
            Timestamp = DateTime.UtcNow
        });
    }

    public void RestoreVersion(int versionNumber, string restoredBy)
    {
        if (versionNumber < 1 || versionNumber > CurrentVersion)
            throw new ArgumentException("Invalid version number", nameof(versionNumber));
        if (string.IsNullOrWhiteSpace(restoredBy))
            throw new ArgumentException("RestoredBy cannot be empty", nameof(restoredBy));

        var previousVersion = CurrentVersion;
        CurrentVersion++;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContentVersionRestoredEvent
        {
            ContentId = ContentId,
            AccountId = AccountId,
            RestoredVersionNumber = versionNumber,
            CurrentVersionNumber = CurrentVersion,
            RestoredBy = restoredBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Schedule(DateTime publishDate, string scheduledBy)
    {
        if (publishDate <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled publish date must be in the future", nameof(publishDate));
        if (string.IsNullOrWhiteSpace(scheduledBy))
            throw new ArgumentException("ScheduledBy cannot be empty", nameof(scheduledBy));

        ScheduledPublishDate = publishDate;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContentScheduledEvent
        {
            ContentId = ContentId,
            AccountId = AccountId,
            ScheduledPublishDate = publishDate,
            ScheduledBy = scheduledBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void CancelSchedule(string cancelledBy)
    {
        if (string.IsNullOrWhiteSpace(cancelledBy))
            throw new ArgumentException("CancelledBy cannot be empty", nameof(cancelledBy));
        if (!ScheduledPublishDate.HasValue)
            throw new InvalidOperationException("No scheduled publish date to cancel");

        var originalDate = ScheduledPublishDate.Value;
        ScheduledPublishDate = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContentScheduleCancelledEvent
        {
            ContentId = ContentId,
            AccountId = AccountId,
            CancelledBy = cancelledBy,
            OriginalScheduledDate = originalDate,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
