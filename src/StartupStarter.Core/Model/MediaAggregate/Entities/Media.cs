using StartupStarter.Core.Model.MediaAggregate.Enums;
using StartupStarter.Core.Model.MediaAggregate.Events;

namespace StartupStarter.Core.Model.MediaAggregate.Entities;

public class Media
{
    public string MediaId { get; private set; }
    public string FileName { get; private set; }
    public string FileType { get; private set; }
    public long FileSize { get; private set; }
    public string UploadedBy { get; private set; }
    public string AccountId { get; private set; }
    public string ProfileId { get; private set; }
    public string StorageLocation { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DeletionType? DeletionType { get; private set; }
    public string ProcessingStatus { get; private set; }

    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private readonly List<string> _categories = new();
    public IReadOnlyCollection<string> Categories => _categories.AsReadOnly();

    private readonly List<string> _outputFormats = new();
    public IReadOnlyCollection<string> OutputFormats => _outputFormats.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private Media()
    {
        MediaId = string.Empty;
        FileName = string.Empty;
        FileType = string.Empty;
        UploadedBy = string.Empty;
        AccountId = string.Empty;
        ProfileId = string.Empty;
        StorageLocation = string.Empty;
        ProcessingStatus = string.Empty;
    }

    public Media(string mediaId, string fileName, string fileType, long fileSize,
        string uploadedBy, string accountId, string profileId, string storageLocation)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
            throw new ArgumentException("Media ID cannot be empty", nameof(mediaId));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        if (string.IsNullOrWhiteSpace(uploadedBy))
            throw new ArgumentException("UploadedBy cannot be empty", nameof(uploadedBy));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));
        if (string.IsNullOrWhiteSpace(storageLocation))
            throw new ArgumentException("Storage location cannot be empty", nameof(storageLocation));

        MediaId = mediaId;
        FileName = fileName;
        FileType = fileType ?? string.Empty;
        FileSize = fileSize;
        UploadedBy = uploadedBy;
        AccountId = accountId;
        ProfileId = profileId;
        StorageLocation = storageLocation;
        UploadedAt = DateTime.UtcNow;
        ProcessingStatus = "Pending";

        AddDomainEvent(new MediaUploadedEvent
        {
            MediaId = MediaId,
            FileName = FileName,
            FileType = FileType,
            FileSize = FileSize,
            UploadedBy = UploadedBy,
            AccountId = AccountId,
            ProfileId = ProfileId,
            StorageLocation = StorageLocation,
            Timestamp = UploadedAt
        });
    }

    public void Update(Dictionary<string, object> updatedFields, string updatedBy)
    {
        if (updatedFields == null || !updatedFields.Any())
            throw new ArgumentException("Updated fields cannot be empty", nameof(updatedFields));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be empty", nameof(updatedBy));

        var previousValues = new Dictionary<string, object>();

        if (updatedFields.ContainsKey("FileName"))
        {
            previousValues["FileName"] = FileName;
            FileName = updatedFields["FileName"].ToString() ?? FileName;
        }

        if (updatedFields.ContainsKey("FileType"))
        {
            previousValues["FileType"] = FileType;
            FileType = updatedFields["FileType"].ToString() ?? FileType;
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MediaUpdatedEvent
        {
            MediaId = MediaId,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            PreviousValues = previousValues,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy, DeletionType deletionType)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy cannot be empty", nameof(deletedBy));

        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletionType = deletionType;

        AddDomainEvent(new MediaDeletedEvent
        {
            MediaId = MediaId,
            FileName = FileName,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            DeletionType = deletionType,
            Timestamp = UpdatedAt.Value
        });
    }

    public void RecordDownload(string downloadedBy)
    {
        if (string.IsNullOrWhiteSpace(downloadedBy))
            throw new ArgumentException("DownloadedBy cannot be empty", nameof(downloadedBy));

        AddDomainEvent(new MediaDownloadedEvent
        {
            MediaId = MediaId,
            FileName = FileName,
            AccountId = AccountId,
            DownloadedBy = downloadedBy,
            Timestamp = DateTime.UtcNow
        });
    }

    public void MarkProcessed(string processingType, List<string> outputFormats, TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(processingType))
            throw new ArgumentException("Processing type cannot be empty", nameof(processingType));

        ProcessingStatus = "Completed";
        UpdatedAt = DateTime.UtcNow;

        if (outputFormats != null && outputFormats.Any())
        {
            _outputFormats.Clear();
            _outputFormats.AddRange(outputFormats);
        }

        AddDomainEvent(new MediaProcessedEvent
        {
            MediaId = MediaId,
            AccountId = AccountId,
            ProcessingType = processingType,
            OutputFormats = outputFormats ?? new List<string>(),
            Duration = duration,
            Timestamp = UpdatedAt.Value
        });
    }

    public void AddTags(List<string> tags, string taggedBy)
    {
        if (tags == null || !tags.Any())
            throw new ArgumentException("Tags cannot be empty", nameof(tags));
        if (string.IsNullOrWhiteSpace(taggedBy))
            throw new ArgumentException("TaggedBy cannot be empty", nameof(taggedBy));

        var newTags = tags.Where(t => !_tags.Contains(t)).ToList();
        if (newTags.Any())
        {
            _tags.AddRange(newTags);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaTaggedEvent
            {
                MediaId = MediaId,
                AccountId = AccountId,
                Tags = newTags,
                TaggedBy = taggedBy,
                Timestamp = UpdatedAt.Value
            });
        }
    }

    public void AddCategories(List<string> categories, string categorizedBy)
    {
        if (categories == null || !categories.Any())
            throw new ArgumentException("Categories cannot be empty", nameof(categories));
        if (string.IsNullOrWhiteSpace(categorizedBy))
            throw new ArgumentException("CategorizedBy cannot be empty", nameof(categorizedBy));

        var newCategories = categories.Where(c => !_categories.Contains(c)).ToList();
        if (newCategories.Any())
        {
            _categories.AddRange(newCategories);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaCategorizedEvent
            {
                MediaId = MediaId,
                AccountId = AccountId,
                Categories = newCategories,
                CategorizedBy = categorizedBy,
                Timestamp = UpdatedAt.Value
            });
        }
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
