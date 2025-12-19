# Media Management - Detailed Design

## Overview

The Media Management module provides comprehensive functionality for uploading, storing, processing, organizing, and delivering media files (images, videos, audio, documents) within the StartupStarter platform. It follows Clean Architecture principles with Domain-Driven Design (DDD) patterns and leverages Azure cloud services for scalable storage and delivery.

## Architecture Principles

- **Clean Architecture**: Separation of concerns across layers (Domain, Application, Infrastructure, API)
- **Domain-Driven Design**: Rich domain models with business logic encapsulation
- **CQRS**: Command Query Responsibility Segregation using MediatR
- **Event-Driven**: Domain events for loose coupling and eventual consistency
- **Cloud-Native**: Azure Blob Storage, CDN, and Service Bus integration

## Domain Model

### Aggregate Root: Media

The `Media` aggregate root is the primary entity that encapsulates all business rules related to media files:

- **Identity**: `MediaId` (Guid)
- **File Information**: FileName, FileType, FileSize, ContentType
- **Storage**: StorageLocation, IsProcessed, ProcessedAt
- **Metadata**: Width, Height, Duration (for images/videos)
- **Organization**: Tags, Categories
- **Ownership**: AccountId, ProfileId, UploadedBy
- **Tracking**: UploadedAt, UpdatedAt, DownloadCount
- **Lifecycle**: IsDeleted, DeletedAt

### Entities

1. **MediaTag**: User-defined tags for organizing and searching media
2. **MediaCategory**: Hierarchical categories for media classification
3. **ProcessedMediaVersion**: Different formats/sizes generated during processing

### Value Objects & Enums

- **FileType**: Image, Video, Audio, Document, Other
- **ProcessingType**: ImageResize, ImageOptimization, VideoTranscode, VideoThumbnail, AudioTranscode
- **DeletionType**: SoftDelete, HardDelete

## Azure Blob Storage Configuration

### Storage Account Setup

```json
{
  "AzureStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=startupstarter;AccountKey=...;EndpointSuffix=core.windows.net",
    "ContainerName": "media",
    "MediaContainerName": "media-files",
    "ProcessedContainerName": "media-processed",
    "ThumbnailContainerName": "media-thumbnails",
    "CdnEndpoint": "https://cdn.startupstarter.com",
    "DefaultSasExpirationMinutes": 60,
    "EnableCdn": true,
    "GeoRedundancy": true,
    "TierStrategy": {
      "HotTier": {
        "Description": "Recently uploaded/frequently accessed media",
        "MaxAge": 30
      },
      "CoolTier": {
        "Description": "Older media with occasional access",
        "MinAge": 30,
        "MaxAge": 90
      },
      "ArchiveTier": {
        "Description": "Rarely accessed historical media",
        "MinAge": 90
      }
    }
  }
}
```

### Container Organization

```
media-files/
  ├── {accountId}/
  │   ├── {mediaId}/
  │   │   ├── original/{filename}
  │   │   └── metadata.json

media-processed/
  ├── {accountId}/
  │   ├── {mediaId}/
  │   │   ├── thumbnails/
  │   │   │   ├── small.jpg (200x200)
  │   │   │   ├── medium.jpg (400x400)
  │   │   │   └── large.jpg (800x800)
  │   │   ├── images/
  │   │   │   ├── webp/
  │   │   │   ├── avif/
  │   │   │   └── optimized/
  │   │   ├── videos/
  │   │   │   ├── 480p.mp4
  │   │   │   ├── 720p.mp4
  │   │   │   ├── 1080p.mp4
  │   │   │   └── thumbnails/
  │   │   └── audio/
  │   │       ├── mp3/
  │   │       ├── aac/
  │   │       └── ogg/
```

### Blob Naming Strategy

```
Pattern: {accountId}/{mediaId}/{category}/{format}/{filename}

Examples:
- Original: abc123/def456/original/photo.jpg
- Thumbnail: abc123/def456/thumbnails/medium.jpg
- Processed: abc123/def456/images/webp/photo.webp
- Video: abc123/def456/videos/720p.mp4
```

### Access Control

1. **Container Access Level**: Private (no anonymous access)
2. **SAS Token Generation**: Time-limited shared access signatures for downloads
3. **RBAC**: Azure AD-based role assignments for administrative access
4. **Storage Account Firewall**: IP restrictions and VNet integration

```csharp
// SAS Token Generation Example
public async Task<string> GetDownloadUrlAsync(
    string storageLocation,
    TimeSpan expirationTime,
    CancellationToken cancellationToken = default)
{
    var blobClient = _blobContainerClient.GetBlobClient(storageLocation);

    var sasBuilder = new BlobSasBuilder
    {
        BlobContainerName = _blobContainerClient.Name,
        BlobName = storageLocation,
        Resource = "b",
        StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
        ExpiresOn = DateTimeOffset.UtcNow.Add(expirationTime)
    };

    sasBuilder.SetPermissions(BlobSasPermissions.Read);

    var sasToken = blobClient.GenerateSasUri(sasBuilder);

    return sasToken.ToString();
}
```

### Lifecycle Management

```json
{
  "rules": [
    {
      "name": "MoveToCoolTier",
      "enabled": true,
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["media-files/"]
        },
        "actions": {
          "baseBlob": {
            "tierToCool": {
              "daysAfterModificationGreaterThan": 30
            },
            "tierToArchive": {
              "daysAfterModificationGreaterThan": 90
            }
          }
        }
      }
    },
    {
      "name": "DeleteSoftDeletedMedia",
      "enabled": true,
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["media-files/deleted/"]
        },
        "actions": {
          "baseBlob": {
            "delete": {
              "daysAfterModificationGreaterThan": 365
            }
          }
        }
      }
    }
  ]
}
```

## Media Processing Workflows

### Image Processing Pipeline

```
Upload → Validate → Store Original → Process → Generate Variants → Update Metadata → Publish Event

Processing Steps:
1. Extract metadata (dimensions, EXIF data)
2. Generate thumbnails (small: 200x200, medium: 400x400, large: 800x800)
3. Optimize original (reduce file size, strip metadata)
4. Convert to modern formats (WebP, AVIF)
5. Store processed variants
6. Update Media aggregate with processing results
```

**Configuration**:

```json
{
  "ImageProcessing": {
    "MaxUploadSizeMB": 50,
    "ThumbnailSizes": [
      { "Name": "small", "Width": 200, "Height": 200 },
      { "Name": "medium", "Width": 400, "Height": 400 },
      { "Name": "large", "Width": 800, "Height": 800 }
    ],
    "OutputFormats": ["webp", "avif", "jpg"],
    "Quality": {
      "webp": 85,
      "avif": 80,
      "jpg": 90
    },
    "OptimizeOriginal": true,
    "StripMetadata": true,
    "PreserveAspectRatio": true
  }
}
```

**Implementation** (using ImageSharp):

```csharp
public async Task<ProcessingResult> ProcessImageAsync(
    Stream imageStream,
    ProcessingType processingType,
    List<string> outputFormats,
    CancellationToken cancellationToken = default)
{
    var stopwatch = Stopwatch.StartNew();
    var processedFiles = new List<ProcessedFile>();

    using var image = await Image.LoadAsync(imageStream, cancellationToken);

    // Extract metadata
    var metadata = new MediaMetadata(
        Width: image.Width,
        Height: image.Height,
        Duration: null,
        Bitrate: null,
        Format: image.Metadata.DecodedImageFormat?.Name
    );

    // Generate thumbnails
    foreach (var size in _config.ThumbnailSizes)
    {
        var thumbnail = image.Clone(ctx => ctx.Resize(size.Width, size.Height));
        var stream = new MemoryStream();
        await thumbnail.SaveAsJpegAsync(stream, cancellationToken);
        stream.Position = 0;

        processedFiles.Add(new ProcessedFile(
            Format: $"thumbnail_{size.Name}",
            FileStream: stream,
            FileSize: stream.Length,
            Width: size.Width,
            Height: size.Height
        ));
    }

    // Convert to modern formats
    foreach (var format in outputFormats)
    {
        var stream = new MemoryStream();
        switch (format.ToLower())
        {
            case "webp":
                await image.SaveAsWebpAsync(stream, new WebpEncoder
                {
                    Quality = _config.Quality.Webp
                }, cancellationToken);
                break;
            case "avif":
                await image.SaveAsAvifAsync(stream, cancellationToken);
                break;
            case "jpg":
                await image.SaveAsJpegAsync(stream, new JpegEncoder
                {
                    Quality = _config.Quality.Jpg
                }, cancellationToken);
                break;
        }

        stream.Position = 0;
        processedFiles.Add(new ProcessedFile(
            Format: format,
            FileStream: stream,
            FileSize: stream.Length,
            Width: image.Width,
            Height: image.Height
        ));
    }

    stopwatch.Stop();

    return new ProcessingResult(
        Success: true,
        ProcessedFiles: processedFiles,
        Duration: stopwatch.Elapsed
    );
}
```

### Video Processing Pipeline

```
Upload → Validate → Store Original → Queue Processing → Transcode → Generate Thumbnails → Store Variants → Update Metadata

Processing Steps:
1. Extract metadata (duration, resolution, codec, bitrate)
2. Generate preview thumbnail
3. Transcode to multiple resolutions (480p, 720p, 1080p)
4. Generate adaptive bitrate streaming manifests (HLS/DASH)
5. Extract audio track
6. Store processed variants
7. Update Media aggregate
```

**Configuration**:

```json
{
  "VideoProcessing": {
    "MaxUploadSizeMB": 500,
    "OutputResolutions": [
      { "Name": "480p", "Width": 854, "Height": 480, "Bitrate": "1000k" },
      { "Name": "720p", "Width": 1280, "Height": 720, "Bitrate": "2500k" },
      { "Name": "1080p", "Width": 1920, "Height": 1080, "Bitrate": "5000k" }
    ],
    "VideoCodec": "libx264",
    "AudioCodec": "aac",
    "ThumbnailCount": 5,
    "ThumbnailInterval": 10,
    "GenerateHLS": true,
    "GenerateDASH": false,
    "MaxProcessingTimeMinutes": 30
  }
}
```

**Implementation** (using FFmpeg):

```csharp
public async Task<ProcessingResult> ProcessVideoAsync(
    Stream videoStream,
    ProcessingType processingType,
    List<string> outputFormats,
    CancellationToken cancellationToken = default)
{
    var stopwatch = Stopwatch.StartNew();
    var processedFiles = new List<ProcessedFile>();

    // Save to temp file (FFmpeg requires file path)
    var tempInputPath = Path.GetTempFileName();
    var tempOutputPath = Path.GetTempPath();

    try
    {
        await using (var fileStream = File.Create(tempInputPath))
        {
            await videoStream.CopyToAsync(fileStream, cancellationToken);
        }

        // Extract metadata
        var mediaInfo = await FFmpeg.GetMediaInfo(tempInputPath);
        var videoStream = mediaInfo.VideoStreams.FirstOrDefault();

        var metadata = new MediaMetadata(
            Width: videoStream?.Width,
            Height: videoStream?.Height,
            Duration: mediaInfo.Duration,
            Bitrate: (int?)mediaInfo.BitRate,
            Format: mediaInfo.Format
        );

        // Generate thumbnail
        var thumbnailPath = Path.Combine(tempOutputPath, "thumbnail.jpg");
        await FFmpeg.Conversions.New()
            .AddParameter($"-i \"{tempInputPath}\" -ss 00:00:01 -vframes 1 \"{thumbnailPath}\"")
            .Start(cancellationToken);

        var thumbnailStream = File.OpenRead(thumbnailPath);
        processedFiles.Add(new ProcessedFile(
            Format: "thumbnail",
            FileStream: thumbnailStream,
            FileSize: thumbnailStream.Length
        ));

        // Transcode to multiple resolutions
        foreach (var resolution in _config.OutputResolutions)
        {
            var outputPath = Path.Combine(tempOutputPath, $"{resolution.Name}.mp4");

            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{tempInputPath}\"")
                .AddParameter($"-vf scale={resolution.Width}:{resolution.Height}")
                .AddParameter($"-b:v {resolution.Bitrate}")
                .AddParameter($"-c:v {_config.VideoCodec}")
                .AddParameter($"-c:a {_config.AudioCodec}")
                .AddParameter($"\"{outputPath}\"")
                .Start(cancellationToken);

            var transcodedStream = File.OpenRead(outputPath);
            processedFiles.Add(new ProcessedFile(
                Format: resolution.Name,
                FileStream: transcodedStream,
                FileSize: transcodedStream.Length,
                Width: resolution.Width,
                Height: resolution.Height
            ));
        }

        stopwatch.Stop();

        return new ProcessingResult(
            Success: true,
            ProcessedFiles: processedFiles,
            Duration: stopwatch.Elapsed
        );
    }
    finally
    {
        // Cleanup temp files
        if (File.Exists(tempInputPath))
            File.Delete(tempInputPath);
    }
}
```

### Audio Processing Pipeline

```
Upload → Validate → Store Original → Process → Normalize → Transcode → Store Variants → Update Metadata

Processing Steps:
1. Extract metadata (duration, bitrate, codec, sample rate)
2. Normalize audio levels
3. Transcode to multiple formats (MP3, AAC, OGG)
4. Generate waveform image
5. Store processed variants
6. Update Media aggregate
```

**Configuration**:

```json
{
  "AudioProcessing": {
    "MaxUploadSizeMB": 100,
    "OutputFormats": [
      { "Format": "mp3", "Bitrate": "192k" },
      { "Format": "aac", "Bitrate": "192k" },
      { "Format": "ogg", "Quality": "6" }
    ],
    "Normalize": true,
    "GenerateWaveform": true,
    "MaxProcessingTimeMinutes": 15
  }
}
```

## CDN Integration

### Azure CDN Configuration

```json
{
  "AzureCdn": {
    "ProfileName": "startupstarter-cdn",
    "EndpointName": "media",
    "CustomDomain": "cdn.startupstarter.com",
    "OriginHostHeader": "startupstarter.blob.core.windows.net",
    "OriginPath": "/media-processed",
    "HttpsOnly": true,
    "QueryStringCachingBehavior": "IgnoreQueryString",
    "CachingRules": [
      {
        "Name": "Images",
        "MatchCondition": "*.{jpg,jpeg,png,gif,webp,avif}",
        "CacheDuration": "7.00:00:00"
      },
      {
        "Name": "Videos",
        "MatchCondition": "*.{mp4,webm,m3u8}",
        "CacheDuration": "30.00:00:00"
      },
      {
        "Name": "Thumbnails",
        "MatchCondition": "*thumbnails*",
        "CacheDuration": "30.00:00:00"
      }
    ],
    "CompressionEnabled": true,
    "GeoFiltering": false
  }
}
```

### CDN Service Implementation

```csharp
public class CdnService : ICdnService
{
    private readonly CdnManagementClient _cdnClient;
    private readonly IConfiguration _config;

    public async Task<string> GetCdnUrlAsync(string blobPath)
    {
        var cdnEndpoint = _config["AzureCdn:CustomDomain"];
        var cdnUrl = $"https://{cdnEndpoint}/{blobPath}";

        return cdnUrl;
    }

    public async Task PurgeCacheAsync(List<string> paths, CancellationToken cancellationToken = default)
    {
        var resourceGroup = _config["AzureCdn:ResourceGroup"];
        var profileName = _config["AzureCdn:ProfileName"];
        var endpointName = _config["AzureCdn:EndpointName"];

        await _cdnClient.Endpoints.PurgeContentAsync(
            resourceGroup,
            profileName,
            endpointName,
            paths,
            cancellationToken);
    }

    public async Task PreloadContentAsync(List<string> paths, CancellationToken cancellationToken = default)
    {
        var resourceGroup = _config["AzureCdn:ResourceGroup"];
        var profileName = _config["AzureCdn:ProfileName"];
        var endpointName = _config["AzureCdn:EndpointName"];

        await _cdnClient.Endpoints.LoadContentAsync(
            resourceGroup,
            profileName,
            endpointName,
            paths,
            cancellationToken);
    }
}
```

### Cache Invalidation Strategy

**When to Purge Cache**:

1. Media file updated/replaced
2. Media deleted (soft or hard)
3. Processed variant regenerated
4. Access permissions changed

**Implementation**:

```csharp
public class MediaUpdatedEventHandler : INotificationHandler<MediaUpdatedDomainEvent>
{
    private readonly ICdnService _cdnService;
    private readonly IMediaRepository _mediaRepository;

    public async Task Handle(MediaUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var media = await _mediaRepository.GetByIdAsync(notification.MediaId, cancellationToken);

        if (media != null)
        {
            var paths = new List<string>
            {
                // Original
                $"{media.AccountId}/{media.MediaId}/original/*",

                // Thumbnails
                $"{media.AccountId}/{media.MediaId}/thumbnails/*",

                // Processed versions
                $"{media.AccountId}/{media.MediaId}/images/*",
                $"{media.AccountId}/{media.MediaId}/videos/*"
            };

            await _cdnService.PurgeCacheAsync(paths, cancellationToken);
        }
    }
}
```

## Background Processing

### Hangfire Job Configuration

```csharp
public class MediaProcessingJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<MediaProcessingJob> _logger;

    [AutomaticRetry(Attempts = 3)]
    [Queue("media-processing")]
    public async Task ProcessMediaAsync(Guid mediaId)
    {
        _logger.LogInformation("Starting media processing for MediaId: {MediaId}", mediaId);

        try
        {
            var command = new ProcessMediaCommand(
                MediaId: mediaId,
                ProcessingType: ProcessingType.ImageResize,
                OutputFormats: new List<string> { "webp", "avif", "jpg" }
            );

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Media processing completed for MediaId: {MediaId} in {Duration}",
                    mediaId,
                    result.ProcessingDuration);
            }
            else
            {
                _logger.LogError("Media processing failed for MediaId: {MediaId}", mediaId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing media for MediaId: {MediaId}", mediaId);
            throw;
        }
    }
}
```

### Job Scheduling

```csharp
public class MediaUploadedEventHandler : INotificationHandler<MediaUploadedDomainEvent>
{
    private readonly IBackgroundJobClient _jobClient;

    public async Task Handle(MediaUploadedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Queue media processing job
        _jobClient.Enqueue<MediaProcessingJob>(job =>
            job.ProcessMediaAsync(notification.MediaId));
    }
}
```

## Security Considerations

1. **File Validation**: Validate file types, sizes, and content before upload
2. **Virus Scanning**: Integrate with antivirus service for uploaded files
3. **Access Control**: Enforce account-based permissions for media access
4. **SAS Token Security**: Time-limited tokens with minimal permissions
5. **HTTPS Only**: All media transfers over encrypted connections
6. **Content Security Policy**: Prevent unauthorized embedding/hotlinking
7. **Rate Limiting**: Prevent abuse of upload/download endpoints

## Performance Optimization

1. **Lazy Loading**: Load thumbnails first, full resolution on demand
2. **Progressive Images**: Use progressive JPEG/WebP for better UX
3. **Responsive Images**: Serve appropriate sizes based on device
4. **CDN Distribution**: Global edge caching for fast delivery
5. **Blob Storage Tiers**: Move infrequently accessed files to cool/archive tiers
6. **Batch Processing**: Process multiple files in parallel
7. **Connection Pooling**: Reuse connections to blob storage

## Monitoring and Observability

### Metrics to Track

- Upload success/failure rates
- Processing times by media type
- Storage consumption by account
- Download counts and bandwidth
- CDN cache hit ratios
- Failed processing jobs
- Average file sizes

### Application Insights

```csharp
public class MediaMetricsService
{
    private readonly TelemetryClient _telemetry;

    public void TrackUpload(MediaUploadedDomainEvent @event)
    {
        _telemetry.TrackEvent("MediaUploaded", new Dictionary<string, string>
        {
            ["MediaId"] = @event.MediaId.ToString(),
            ["FileType"] = @event.FileType,
            ["AccountId"] = @event.AccountId.ToString()
        }, new Dictionary<string, double>
        {
            ["FileSizeBytes"] = @event.FileSize
        });
    }

    public void TrackProcessing(MediaProcessedDomainEvent @event)
    {
        _telemetry.TrackMetric("ProcessingDuration", @event.ProcessingDuration.TotalSeconds, new Dictionary<string, string>
        {
            ["ProcessingType"] = @event.ProcessingType,
            ["MediaId"] = @event.MediaId.ToString()
        });
    }
}
```

## Future Enhancements

1. **AI-Powered Features**:
   - Auto-tagging using computer vision
   - Object detection and facial recognition
   - Content moderation and NSFW detection
   - Smart cropping and focal point detection

2. **Advanced Processing**:
   - Video editing capabilities (trim, merge, filters)
   - Watermarking
   - Format conversion on-demand
   - Live streaming support

3. **Collaboration**:
   - Media commenting and annotations
   - Version control for media files
   - Approval workflows

4. **Analytics**:
   - Usage analytics dashboard
   - Popular content insights
   - Storage optimization recommendations

## References

- [Azure Blob Storage Documentation](https://docs.microsoft.com/en-us/azure/storage/blobs/)
- [Azure CDN Documentation](https://docs.microsoft.com/en-us/azure/cdn/)
- [ImageSharp Documentation](https://docs.sixlabors.com/articles/imagesharp/)
- [FFmpeg Documentation](https://ffmpeg.org/documentation.html)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
