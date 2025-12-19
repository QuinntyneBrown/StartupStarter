# Profile Management Events

## Profile.Created
**Description**: Fired when a new profile is created within an account (Profile belongs to one Account)

**Payload**:
- ProfileId: string
- ProfileName: string
- AccountId: string (the single account this profile belongs to)
- CreatedBy: string (UserId)
- ProfileType: enum (Personal, Project, Department, Team)
- IsDefault: bool
- Timestamp: DateTime

---

## Profile.Updated
**Description**: Fired when profile information is updated

**Payload**:
- ProfileId: string
- AccountId: string
- UpdatedBy: string (UserId)
- UpdatedFields: Dictionary<string, object>
- PreviousValues: Dictionary<string, object>
- Timestamp: DateTime

---

## Profile.Deleted
**Description**: Fired when a profile is deleted

**Payload**:
- ProfileId: string
- AccountId: string
- DeletedBy: string (UserId or AdminId)
- DashboardCount: int
- Timestamp: DateTime

---

## Profile.Avatar.Updated
**Description**: Fired when a profile avatar/image is changed

**Payload**:
- ProfileId: string
- AccountId: string
- UpdatedBy: string (UserId)
- PreviousAvatarUrl: string
- NewAvatarUrl: string
- Timestamp: DateTime

---

## Profile.Preferences.Updated
**Description**: Fired when profile preferences are modified

**Payload**:
- ProfileId: string
- AccountId: string
- UpdatedBy: string (UserId)
- PreferenceCategory: string
- UpdatedPreferences: Dictionary<string, object>
- Timestamp: DateTime

---

## Profile.Shared
**Description**: Fired when a profile is shared with other users within the same account

**Payload**:
- ProfileId: string
- AccountId: string
- OwnerUserId: string
- SharedWithUserIds: List<string>
- PermissionLevel: enum (View, Edit, Admin)
- SharedBy: string (UserId)
- Timestamp: DateTime

---

## Profile.ShareRevoked
**Description**: Fired when profile sharing is revoked from users

**Payload**:
- ProfileId: string
- AccountId: string
- OwnerUserId: string
- RevokedFromUserIds: List<string>
- RevokedBy: string (UserId)
- Timestamp: DateTime

---

## Profile.SetAsDefault
**Description**: Fired when a profile is set as the default profile for a user

**Payload**:
- ProfileId: string
- AccountId: string
- UserId: string
- PreviousDefaultProfileId: string (optional)
- Timestamp: DateTime

---

## Profile.Dashboard.Added
**Description**: Fired when a dashboard is added to the profile

**Payload**:
- ProfileId: string
- AccountId: string
- DashboardId: string
- AddedBy: string (UserId)
- Timestamp: DateTime

---

## Profile.Dashboard.Removed
**Description**: Fired when a dashboard is removed from the profile

**Payload**:
- ProfileId: string
- AccountId: string
- DashboardId: string
- RemovedBy: string (UserId)
- Timestamp: DateTime
