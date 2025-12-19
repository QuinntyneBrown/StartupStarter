# Content Events

## Content.Created
**Description**: Fired when new content is created in the CMS

**Payload**:
- ContentId: string
- ContentType: string
- Title: string
- AuthorId: string (UserId)
- AccountId: string
- ProfileId: string (optional - if associated with specific profile)
- Status: enum (Draft, Review, Published)
- Timestamp: DateTime

---

## Content.Updated
**Description**: Fired when content is modified

**Payload**:
- ContentId: string
- ContentType: string
- AccountId: string
- UpdatedBy: string (UserId)
- UpdatedFields: Dictionary<string, object>
- VersionNumber: int
- Timestamp: DateTime

---

## Content.Deleted
**Description**: Fired when content is deleted

**Payload**:
- ContentId: string
- ContentType: string
- AccountId: string
- DeletedBy: string (UserId)
- DeletionType: enum (SoftDelete, HardDelete)
- Timestamp: DateTime

---

## Content.Published
**Description**: Fired when content is published

**Payload**:
- ContentId: string
- ContentType: string
- AccountId: string
- PublishedBy: string (UserId)
- PublishDate: DateTime
- ScheduledPublish: bool
- Timestamp: DateTime

---

## Content.Unpublished
**Description**: Fired when published content is unpublished

**Payload**:
- ContentId: string
- ContentType: string
- AccountId: string
- UnpublishedBy: string (UserId)
- Reason: string
- Timestamp: DateTime

---

## Content.StatusChanged
**Description**: Fired when content workflow status changes

**Payload**:
- ContentId: string
- ContentType: string
- AccountId: string
- PreviousStatus: string
- NewStatus: string
- ChangedBy: string (UserId)
- Timestamp: DateTime

---

## Content.VersionCreated
**Description**: Fired when a new version of content is created

**Payload**:
- ContentId: string
- AccountId: string
- VersionNumber: int
- CreatedBy: string (UserId)
- ChangeDescription: string
- Timestamp: DateTime

---

## Content.VersionRestored
**Description**: Fired when a previous version is restored

**Payload**:
- ContentId: string
- AccountId: string
- RestoredVersionNumber: int
- CurrentVersionNumber: int
- RestoredBy: string (UserId)
- Timestamp: DateTime

---

## Content.Scheduled
**Description**: Fired when content is scheduled for future publication

**Payload**:
- ContentId: string
- AccountId: string
- ScheduledPublishDate: DateTime
- ScheduledBy: string (UserId)
- Timestamp: DateTime

---

## Content.ScheduleCancelled
**Description**: Fired when a scheduled publication is cancelled

**Payload**:
- ContentId: string
- AccountId: string
- CancelledBy: string (UserId)
- OriginalScheduledDate: DateTime
- Timestamp: DateTime
