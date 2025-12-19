# Media Domain Models

This document defines the C# models needed to implement the media management events following Clean Architecture principles with MediatR.

## Domain Entities

### Media (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class Media : AggregateRoot
    {
        public Guid MediaId { get; private set; }
        public string FileName { get; private set; }
        public FileType FileType { get; private set; }
        public long FileSize { get; private set; }
        public string StorageLocation { get; private set; }
        public string? ContentType { get; private set; }
        public int? Width { get; private set; }
        public int? Height { get; private set; }
        public TimeSpan? Duration { get; private set; }
        public Guid UploadedBy { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid? ProfileId { get; private set; }
        public bool IsProcessed { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public int DownloadCount { get; private set; }

        // Navigation properties
        public virtual User Uploader { get; private set; }
        public virtual Account Account { get; private set; }
        public virtual Profile? Profile { get; private set; }
        public virtual ICollection<MediaTag> Tags { get; private set; } = new List<MediaTag>();
        public virtual ICollection<MediaCategory> Categories { get; private set; } = new List<MediaCategory>();
        public virtual ICollection<ProcessedMediaVersion> ProcessedVersions { get; private set; } = new List<ProcessedMediaVersion>();

        private Media() { } // EF Core

        public static Media Create(
            string fileName,
            FileType fileType,
            long fileSize,
            string storageLocation,
            Guid uploadedBy,
            Guid accountId,
            Guid? profileId = null,
            string? contentType = null)
        {
            var media = new Media
            {
                MediaId = Guid.NewGuid(),
                FileName = fileName,
                FileType = fileType,
                FileSize = fileSize,
                StorageLocation = storageLocation,
                ContentType = contentType,
                UploadedBy = uploadedBy,
                AccountId = accountId,
                ProfileId = profileId,
                IsProcessed = false,
                UploadedAt = DateTime.UtcNow,
                DownloadCount = 0
            };

            media.AddDomainEvent(new MediaUploadedDomainEvent(
                media.MediaId,
                media.FileName,
                media.FileType.ToString(),
                media.FileSize,
                media.UploadedBy,
                media.AccountId,
                media.ProfileId,
                media.StorageLocation));

            return media;
        }

        public void Update(Dictionary<string, object> updatedFields, Guid updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaUpdatedDomainEvent(
                MediaId,
                AccountId,
                updatedBy,
                updatedFields));
        }

        public void MarkAsProcessed(
            ProcessingType processingType,
            List<string> outputFormats,
            TimeSpan processingDuration,
            int? width = null,
            int? height = null,
            TimeSpan? duration = null)
        {
            IsProcessed = true;
            ProcessedAt = DateTime.UtcNow;
            Width = width ?? Width;
            Height = height ?? Height;
            Duration = duration ?? Duration;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaProcessedDomainEvent(
                MediaId,
                AccountId,
                processingType.ToString(),
                outputFormats,
                processingDuration));
        }

        public void AddTags(List<string> tags, Guid taggedBy)
        {
            foreach (var tag in tags)
            {
                if (!Tags.Any(t => t.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    Tags.Add(MediaTag.Create(MediaId, tag, taggedBy));
                }
            }

            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaTaggedDomainEvent(
                MediaId,
                AccountId,
                tags,
                taggedBy));
        }

        public void RemoveTag(string tagName)
        {
            var tag = Tags.FirstOrDefault(t => t.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (tag != null)
            {
                Tags.Remove(tag);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void AssignCategories(List<string> categories, Guid categorizedBy)
        {
            foreach (var category in categories)
            {
                if (!Categories.Any(c => c.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase)))
                {
                    Categories.Add(MediaCategory.Create(MediaId, category, categorizedBy));
                }
            }

            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaCategorizedDomainEvent(
                MediaId,
                AccountId,
                categories,
                categorizedBy));
        }

        public void RemoveCategory(string categoryName)
        {
            var category = Categories.FirstOrDefault(c => c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (category != null)
            {
                Categories.Remove(category);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void IncrementDownloadCount(Guid downloadedBy)
        {
            DownloadCount++;

            AddDomainEvent(new MediaDownloadedDomainEvent(
                MediaId,
                AccountId,
                downloadedBy));
        }

        public void Delete(Guid deletedBy, DeletionType deletionType)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaDeletedDomainEvent(
                MediaId,
                FileName,
                AccountId,
                deletedBy,
                deletionType));
        }
    }
}
```

### MediaTag (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class MediaTag
    {
        public Guid TagId { get; private set; }
        public Guid MediaId { get; private set; }
        public string TagName { get; private set; }
        public Guid TaggedBy { get; private set; }
        public DateTime TaggedAt { get; private set; }

        // Navigation
        public virtual Media Media { get; private set; }
        public virtual User Tagger { get; private set; }

        private MediaTag() { } // EF Core

        public static MediaTag Create(Guid mediaId, string tagName, Guid taggedBy)
        {
            return new MediaTag
            {
                TagId = Guid.NewGuid(),
                MediaId = mediaId,
                TagName = tagName.Trim(),
                TaggedBy = taggedBy,
                TaggedAt = DateTime.UtcNow
            };
        }
    }
}
```

### MediaCategory (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class MediaCategory
    {
        public Guid CategoryId { get; private set; }
        public Guid MediaId { get; private set; }
        public string CategoryName { get; private set; }
        public Guid CategorizedBy { get; private set; }
        public DateTime CategorizedAt { get; private set; }

        // Navigation
        public virtual Media Media { get; private set; }
        public virtual User Categorizer { get; private set; }

        private MediaCategory() { } // EF Core

        public static MediaCategory Create(Guid mediaId, string categoryName, Guid categorizedBy)
        {
            return new MediaCategory
            {
                CategoryId = Guid.NewGuid(),
                MediaId = mediaId,
                CategoryName = categoryName.Trim(),
                CategorizedBy = categorizedBy,
                CategorizedAt = DateTime.UtcNow
            };
        }
    }
}
```

### ProcessedMediaVersion (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class ProcessedMediaVersion
    {
        public Guid VersionId { get; private set; }
        public Guid MediaId { get; private set; }
        public ProcessingType ProcessingType { get; private set; }
        public string OutputFormat { get; private set; }
        public string StorageLocation { get; private set; }
        public long FileSize { get; private set; }
        public int? Width { get; private set; }
        public int? Height { get; private set; }
        public int? Bitrate { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation
        public virtual Media Media { get; private set; }

        private ProcessedMediaVersion() { } // EF Core

        public static ProcessedMediaVersion Create(
            Guid mediaId,
            ProcessingType processingType,
            string outputFormat,
            string storageLocation,
            long fileSize,
            int? width = null,
            int? height = null,
            int? bitrate = null)
        {
            return new ProcessedMediaVersion
            {
                VersionId = Guid.NewGuid(),
                MediaId = mediaId,
                ProcessingType = processingType,
                OutputFormat = outputFormat,
                StorageLocation = storageLocation,
                FileSize = fileSize,
                Width = width,
                Height = height,
                Bitrate = bitrate,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
```

## Enumerations

### FileType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum FileType
    {
        Image = 0,
        Video = 1,
        Audio = 2,
        Document = 3,
        Other = 99
    }
}
```

### ProcessingType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ProcessingType
    {
        None = 0,
        ImageResize = 1,
        ImageOptimization = 2,
        VideoTranscode = 3,
        VideoThumbnail = 4,
        AudioTranscode = 5,
        DocumentConversion = 6
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

### MediaUploadedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaUploadedDomainEvent(
        Guid MediaId,
        string FileName,
        string FileType,
        long FileSize,
        Guid UploadedBy,
        Guid AccountId,
        Guid? ProfileId,
        string StorageLocation) : DomainEvent;
}
```

### MediaUpdatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaUpdatedDomainEvent(
        Guid MediaId,
        Guid AccountId,
        Guid UpdatedBy,
        Dictionary<string, object> UpdatedFields) : DomainEvent;
}
```

### MediaDeletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaDeletedDomainEvent(
        Guid MediaId,
        string FileName,
        Guid AccountId,
        Guid DeletedBy,
        DeletionType DeletionType) : DomainEvent;
}
```

### MediaDownloadedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaDownloadedDomainEvent(
        Guid MediaId,
        Guid AccountId,
        Guid DownloadedBy) : DomainEvent;
}
```

### MediaProcessedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaProcessedDomainEvent(
        Guid MediaId,
        Guid AccountId,
        string ProcessingType,
        List<string> OutputFormats,
        TimeSpan ProcessingDuration) : DomainEvent;
}
```

### MediaTaggedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaTaggedDomainEvent(
        Guid MediaId,
        Guid AccountId,
        List<string> Tags,
        Guid TaggedBy) : DomainEvent;
}
```

### MediaCategorizedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MediaCategorizedDomainEvent(
        Guid MediaId,
        Guid AccountId,
        List<string> Categories,
        Guid CategorizedBy) : DomainEvent;
}
```

## MediatR Commands

### UploadMediaCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record UploadMediaCommand(
        string FileName,
        FileType FileType,
        long FileSize,
        Stream FileStream,
        Guid UploadedBy,
        Guid AccountId,
        Guid? ProfileId = null,
        string? ContentType = null) : IRequest<UploadMediaResponse>;

    public record UploadMediaResponse(
        Guid MediaId,
        string StorageLocation,
        DateTime UploadedAt);
}
```

### UpdateMediaCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record UpdateMediaCommand(
        Guid MediaId,
        Dictionary<string, object> UpdatedFields,
        Guid UpdatedBy) : IRequest<Unit>;
}
```

### DeleteMediaCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record DeleteMediaCommand(
        Guid MediaId,
        Guid DeletedBy,
        DeletionType DeletionType = DeletionType.SoftDelete) : IRequest<Unit>;
}
```

### ProcessMediaCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record ProcessMediaCommand(
        Guid MediaId,
        ProcessingType ProcessingType,
        List<string> OutputFormats) : IRequest<ProcessMediaResponse>;

    public record ProcessMediaResponse(
        Guid MediaId,
        bool Success,
        List<string> GeneratedVersions,
        TimeSpan ProcessingDuration);
}
```

### TagMediaCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record TagMediaCommand(
        Guid MediaId,
        List<string> Tags,
        Guid TaggedBy) : IRequest<Unit>;
}
```

### RemoveMediaTagCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record RemoveMediaTagCommand(
        Guid MediaId,
        string TagName) : IRequest<Unit>;
}
```

### CategorizeMediaCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record CategorizeMediaCommand(
        Guid MediaId,
        List<string> Categories,
        Guid CategorizedBy) : IRequest<Unit>;
}
```

### RemoveMediaCategoryCommand

```csharp
namespace StartupStarter.Application.Media.Commands
{
    public record RemoveMediaCategoryCommand(
        Guid MediaId,
        string CategoryName) : IRequest<Unit>;
}
```

## MediatR Queries

### GetMediaByIdQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetMediaByIdQuery(Guid MediaId) : IRequest<MediaDto?>;
}
```

### GetMediaByAccountQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetMediaByAccountQuery(
        Guid AccountId,
        FileType? FileType = null,
        bool? IsProcessed = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<MediaDto>>;
}
```

### GetMediaByProfileQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetMediaByProfileQuery(
        Guid ProfileId,
        FileType? FileType = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<MediaDto>>;
}
```

### GetMediaByTagQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetMediaByTagQuery(
        Guid AccountId,
        string TagName,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<MediaDto>>;
}
```

### GetMediaByCategoryQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetMediaByCategoryQuery(
        Guid AccountId,
        string CategoryName,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<MediaDto>>;
}
```

### GetMediaDownloadUrlQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetMediaDownloadUrlQuery(
        Guid MediaId,
        Guid RequestedBy,
        TimeSpan? ExpirationTime = null) : IRequest<MediaDownloadUrlDto>;
}
```

### GetProcessedMediaVersionsQuery

```csharp
namespace StartupStarter.Application.Media.Queries
{
    public record GetProcessedMediaVersionsQuery(
        Guid MediaId) : IRequest<List<ProcessedMediaVersionDto>>;
}
```

## DTOs

### MediaDto

```csharp
namespace StartupStarter.Application.Media.DTOs
{
    public record MediaDto(
        Guid MediaId,
        string FileName,
        FileType FileType,
        long FileSize,
        string StorageLocation,
        string? ContentType,
        int? Width,
        int? Height,
        TimeSpan? Duration,
        Guid UploadedBy,
        string UploaderName,
        Guid AccountId,
        Guid? ProfileId,
        bool IsProcessed,
        DateTime? ProcessedAt,
        DateTime UploadedAt,
        DateTime? UpdatedAt,
        int DownloadCount,
        List<string> Tags,
        List<string> Categories);
}
```

### ProcessedMediaVersionDto

```csharp
namespace StartupStarter.Application.Media.DTOs
{
    public record ProcessedMediaVersionDto(
        Guid VersionId,
        ProcessingType ProcessingType,
        string OutputFormat,
        string StorageLocation,
        long FileSize,
        int? Width,
        int? Height,
        int? Bitrate,
        DateTime CreatedAt);
}
```

### MediaDownloadUrlDto

```csharp
namespace StartupStarter.Application.Media.DTOs
{
    public record MediaDownloadUrlDto(
        Guid MediaId,
        string DownloadUrl,
        DateTime ExpiresAt);
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

### IMediaRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IMediaRepository
    {
        Task<Media?> GetByIdAsync(Guid mediaId, CancellationToken cancellationToken = default);
        Task<List<Media>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<PaginatedList<Media>> GetPagedByAccountIdAsync(
            Guid accountId,
            FileType? fileType,
            bool? isProcessed,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<Media>> GetPagedByProfileIdAsync(
            Guid profileId,
            FileType? fileType,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<Media>> GetByTagAsync(
            Guid accountId,
            string tagName,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<Media>> GetByCategoryAsync(
            Guid accountId,
            string categoryName,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<ProcessedMediaVersion>> GetProcessedVersionsAsync(
            Guid mediaId,
            CancellationToken cancellationToken = default);
        Task AddAsync(Media media, CancellationToken cancellationToken = default);
        Task UpdateAsync(Media media, CancellationToken cancellationToken = default);
        Task DeleteAsync(Media media, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

## Service Interfaces

### IMediaStorageService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IMediaStorageService
    {
        Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            Guid accountId,
            CancellationToken cancellationToken = default);

        Task<Stream> DownloadAsync(
            string storageLocation,
            CancellationToken cancellationToken = default);

        Task<string> GetDownloadUrlAsync(
            string storageLocation,
            TimeSpan expirationTime,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string storageLocation,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            string storageLocation,
            CancellationToken cancellationToken = default);
    }
}
```

### IMediaProcessingService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IMediaProcessingService
    {
        Task<ProcessingResult> ProcessImageAsync(
            Stream imageStream,
            ProcessingType processingType,
            List<string> outputFormats,
            CancellationToken cancellationToken = default);

        Task<ProcessingResult> ProcessVideoAsync(
            Stream videoStream,
            ProcessingType processingType,
            List<string> outputFormats,
            CancellationToken cancellationToken = default);

        Task<ProcessingResult> ProcessAudioAsync(
            Stream audioStream,
            ProcessingType processingType,
            List<string> outputFormats,
            CancellationToken cancellationToken = default);

        Task<MediaMetadata> ExtractMetadataAsync(
            Stream mediaStream,
            FileType fileType,
            CancellationToken cancellationToken = default);
    }

    public record ProcessingResult(
        bool Success,
        List<ProcessedFile> ProcessedFiles,
        TimeSpan Duration,
        string? ErrorMessage = null);

    public record ProcessedFile(
        string Format,
        Stream FileStream,
        long FileSize,
        int? Width = null,
        int? Height = null,
        int? Bitrate = null);

    public record MediaMetadata(
        int? Width,
        int? Height,
        TimeSpan? Duration,
        int? Bitrate,
        string? Format);
}
```
