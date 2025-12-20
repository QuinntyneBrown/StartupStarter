# Content Management - Detailed Design

## Overview
CMS functionality for creating, managing, and publishing content.

## Aggregates
- **ContentAggregate**: Content items with versioning

## Key Features
- Content versioning (full history)
- Draft/Review/Published workflow
- Scheduled publishing
- Content types (extensible)
- Soft delete with recovery

## Dependencies
- **AccountAggregate**: Content belongs to account
- **ProfileAggregate**: Content can be profile-specific
- **WorkflowAggregate**: Approval workflows
- **MediaAggregate**: Content can reference media

## Business Rules
1. Only authors and admins can edit content
2. Published content requires approval
3. Version created on every update
4. Can restore any previous version
5. Scheduled publish executes at exact time
6. Unpublishing requires reason

## Data Model
**Contents Table**
- ContentId, Title, Body, ContentType
- AccountId, ProfileId, AuthorId
- Status, CurrentVersion
- PublishedAt, ScheduledPublishDate
- Soft delete support

**ContentVersions Table**
- ContentVersionId, ContentId
- VersionNumber, Title, Body
- CreatedBy, ChangeDescription
- CreatedAt

## Sequence: Publish Content
```
User → PublishContentCommand
→ Validate user permissions
→ Check content status (must be Approved)
→ If scheduled: Queue for later
→ If immediate: Change status to Published
→ Record PublishedAt timestamp
→ Create new version
→ Publish ContentPublishedEvent
→ Notify stakeholders
```

## API Endpoints
- POST /api/content - Create content
- GET /api/content/{id} - Get content
- PUT /api/content/{id} - Update content
- DELETE /api/content/{id} - Delete content
- POST /api/content/{id}/publish - Publish
- POST /api/content/{id}/unpublish - Unpublish
- GET /api/content/{id}/versions - List versions
- POST /api/content/{id}/restore/{version} - Restore version
- POST /api/content/{id}/schedule - Schedule publish

## Performance
- Full-text search on title/body
- Caching of published content
- CDN integration for public content
- Lazy loading of version history
