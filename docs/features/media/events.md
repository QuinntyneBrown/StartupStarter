# Media Events

## Media.Uploaded
**Description**: Fired when a media file is uploaded

**Payload**:
- MediaId: string
- FileName: string
- FileType: string
- FileSize: long
- UploadedBy: string (UserId)
- AccountId: string
- ProfileId: string (optional - if uploaded to specific profile)
- StorageLocation: string
- Timestamp: DateTime

---

## Media.Updated
**Description**: Fired when media metadata is updated

**Payload**:
- MediaId: string
- AccountId: string
- UpdatedBy: string (UserId)
- UpdatedFields: Dictionary<string, object>
- Timestamp: DateTime

---

## Media.Deleted
**Description**: Fired when media is deleted

**Payload**:
- MediaId: string
- FileName: string
- AccountId: string
- DeletedBy: string (UserId)
- DeletionType: enum (SoftDelete, HardDelete)
- Timestamp: DateTime

---

## Media.Downloaded
**Description**: Fired when media is downloaded

**Payload**:
- MediaId: string
- AccountId: string
- DownloadedBy: string (UserId)
- Timestamp: DateTime

---

## Media.Processed
**Description**: Fired when media processing (resize, transcode, etc.) completes

**Payload**:
- MediaId: string
- AccountId: string
- ProcessingType: string
- OutputFormats: List<string>
- ProcessingDuration: TimeSpan
- Timestamp: DateTime

---

## Media.Tagged
**Description**: Fired when tags are added to media

**Payload**:
- MediaId: string
- AccountId: string
- Tags: List<string>
- TaggedBy: string (UserId)
- Timestamp: DateTime

---

## Media.Categorized
**Description**: Fired when media is assigned to categories

**Payload**:
- MediaId: string
- AccountId: string
- Categories: List<string>
- CategorizedBy: string (UserId)
- Timestamp: DateTime
