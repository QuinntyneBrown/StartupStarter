# Media Management Models

## Core Aggregate

### MediaAggregate

Located in: `StartupStarter.Core\Model\MediaAggregate\`

#### Folder Structure
```
MediaAggregate/
├── Entities/
│   ├── Media.cs
│   └── MediaTag.cs
├── Enums/
│   └── DeletionType.cs
└── Events/
    ├── MediaUploadedEvent.cs
    ├── MediaUpdatedEvent.cs
    ├── MediaDeletedEvent.cs
    ├── MediaDownloadedEvent.cs
    ├── MediaProcessedEvent.cs
    ├── MediaTaggedEvent.cs
    └── MediaCategorizedEvent.cs
```

#### Entities

Located in: `StartupStarter.Core\Model\MediaAggregate\Entities\`

**Media.cs** (Aggregate Root)
```csharp
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

    public void Update(Dictionary<string, object> updatedFields, string updatedBy);
    public void Delete(string deletedBy, DeletionType deletionType);
    public void RecordDownload(string downloadedBy);
    public void MarkProcessed(string processingType, List<string> outputFormats, TimeSpan duration);
    public void AddTags(List<string> tags, string taggedBy);
    public void AddCategories(List<string> categories, string categorizedBy);
}
```

#### Enums

Located in: `StartupStarter.Core\Model\MediaAggregate\Enums\`

**DeletionType.cs**
```csharp
public enum DeletionType
{
    SoftDelete,
    HardDelete
}
```

#### Domain Events

Located in: `StartupStarter.Core\Model\MediaAggregate\Events\`

**MediaUploadedEvent.cs**
```csharp
public class MediaUploadedEvent : DomainEvent
{
    public string MediaId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public string UploadedBy { get; set; }
    public string AccountId { get; set; }
    public string ProfileId { get; set; }
    public string StorageLocation { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**MediaUpdatedEvent.cs**, **MediaDeletedEvent.cs**, **MediaDownloadedEvent.cs**, **MediaProcessedEvent.cs**, **MediaTaggedEvent.cs**, **MediaCategorizedEvent.cs**

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\Media\Commands\`

**UploadMediaCommand.cs**
```csharp
public class UploadMediaCommand : IRequest<MediaDto>
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public Stream FileStream { get; set; }
    public string UploadedBy { get; set; }
    public string AccountId { get; set; }
    public string ProfileId { get; set; }
}
```

**UpdateMediaCommand.cs**, **DeleteMediaCommand.cs**, **TagMediaCommand.cs**, **CategorizeMediaCommand.cs**
