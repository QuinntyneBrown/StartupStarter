# Dashboard Management Events

## Dashboard.Created
**Description**: Fired when a new dashboard is created for a profile

**Payload**:
- DashboardId: string
- DashboardName: string
- ProfileId: string
- AccountId: string
- CreatedBy: string (UserId)
- IsDefault: bool
- Template: string (optional)
- Timestamp: DateTime

---

## Dashboard.Updated
**Description**: Fired when dashboard properties are updated

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- UpdatedFields: Dictionary<string, object>
- PreviousValues: Dictionary<string, object>
- UpdatedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Deleted
**Description**: Fired when a dashboard is deleted

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- DeletedBy: string (UserId)
- CardCount: int
- Timestamp: DateTime

---

## Dashboard.Cloned
**Description**: Fired when a dashboard is duplicated/cloned

**Payload**:
- SourceDashboardId: string
- NewDashboardId: string
- ProfileId: string
- AccountId: string
- ClonedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.SetAsDefault
**Description**: Fired when a dashboard is set as the default for a profile

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- PreviousDefaultDashboardId: string (optional)
- SetBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Shared
**Description**: Fired when a dashboard is shared with other users within the account

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- OwnerUserId: string
- SharedWithUserIds: List<string>
- PermissionLevel: enum (View, Edit)
- Timestamp: DateTime

---

## Dashboard.ShareRevoked
**Description**: Fired when dashboard sharing is revoked

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- OwnerUserId: string
- RevokedFromUserIds: List<string>
- Timestamp: DateTime

---

## Dashboard.Card.Added
**Description**: Fired when a card is added to a dashboard

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- CardId: string
- CardType: string
- Position: object { Row: int, Column: int, Width: int, Height: int }
- AddedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Card.Updated
**Description**: Fired when a dashboard card is updated

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- CardId: string
- UpdatedFields: Dictionary<string, object>
- PreviousValues: Dictionary<string, object>
- UpdatedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Card.Removed
**Description**: Fired when a card is removed from a dashboard

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- CardId: string
- RemovedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Card.Repositioned
**Description**: Fired when cards are rearranged on a dashboard

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- CardId: string
- PreviousPosition: object { Row: int, Column: int, Width: int, Height: int }
- NewPosition: object { Row: int, Column: int, Width: int, Height: int }
- RepositionedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Layout.Changed
**Description**: Fired when the overall dashboard layout changes

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- LayoutType: enum (Grid, Masonry, Freeform)
- ChangedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Exported
**Description**: Fired when a dashboard is exported

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- ExportFormat: enum (JSON, PDF, Image)
- ExportedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.Imported
**Description**: Fired when a dashboard is imported into a profile

**Payload**:
- DashboardId: string
- ProfileId: string
- AccountId: string
- SourceFormat: enum (JSON, Template)
- ImportedBy: string (UserId)
- Timestamp: DateTime

---

## Dashboard.MovedToProfile
**Description**: Fired when a dashboard is moved from one profile to another within the same account

**Payload**:
- DashboardId: string
- PreviousProfileId: string
- NewProfileId: string
- AccountId: string
- MovedBy: string (UserId)
- Timestamp: DateTime
