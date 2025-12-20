# Media Management - Detailed Design

## Overview
Media file upload, storage, processing, and organization.

## Aggregates
- **MediaAggregate**: Media files with metadata

## Key Features
- File upload (images, videos, documents)
- Cloud storage integration (Azure Blob, S3)
- Image processing (resize, crop, thumbnails)
- Video transcoding
- Tagging and categorization
- Download tracking
- Soft delete with recovery

## Dependencies
- **AccountAggregate**: Media belongs to account
- **ProfileAggregate**: Can be profile-specific
- **Storage Service**: Azure Blob Storage / S3
- **Processing Service**: Image/Video processing

## Business Rules
1. Max file size: 100MB per file
2. Supported formats: Images (JPG, PNG, GIF, WebP), Videos (MP4, MOV), Docs (PDF)
3. Virus scanning before storage
4. Generate thumbnails for images
5. Automatic format conversion for optimization
6. Tags and categories for organization
7. Usage tracking for billing

## Data Model
**Media Table**
- MediaId, FileName, FileType
- FileSize, AccountId, ProfileId
- UploadedBy, StorageLocation
- ProcessingStatus
- Tags (JSON array)
- Categories (JSON array)
- UploadedAt, DeletedAt

## Storage Structure
```
/account-{accountId}/
  /media/
    /{mediaId}/
      /original.{ext}
      /thumbnail.jpg
      /processed/
        /small.{ext}
        /medium.{ext}
        /large.{ext}
```

## Sequence: Upload Media
```
User → UploadMediaCommand (with file stream)
→ Validate file type and size
→ Scan for viruses
→ Generate unique MediaId
→ Upload to blob storage
→ Create Media aggregate
→ Queue processing job
→ Save to database
→ Publish MediaUploadedEvent
→ Return MediaDto

Background Job:
→ Process image (resize, optimize)
→ Generate thumbnails
→ Update processing status
→ Publish MediaProcessedEvent
```

## API Endpoints
- POST /api/media/upload - Upload media
- GET /api/media/{id} - Get media metadata
- GET /api/media/{id}/download - Download media
- PUT /api/media/{id} - Update metadata
- DELETE /api/media/{id} - Delete media
- POST /api/media/{id}/tags - Add tags
- POST /api/media/{id}/categories - Add categories
- GET /api/media/search - Search media

## Processing Pipeline
1. **Upload**: Store original file
2. **Virus Scan**: ClamAV integration
3. **Metadata Extraction**: EXIF, duration, etc.
4. **Image Processing**:
   - Generate thumbnails (100x100, 300x300)
   - Resize variants (small, medium, large)
   - Optimize compression
   - WebP conversion
5. **Video Processing**:
   - Generate thumbnail from first frame
   - Transcode to web-friendly format
   - Generate multiple quality levels

## Security
- Signed URLs for downloads (expiring)
- Access control per media file
- Virus scanning mandatory
- Content-Type validation
- File extension validation

## Performance
- CDN integration for public media
- Lazy loading thumbnails
- Paginated media lists
- Async processing queue
- Caching metadata
