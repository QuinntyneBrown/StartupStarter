
# Account Management

## Account.Created
**Description**: Fired when a new account (organization/tenant) is created

**Payload**:
- AccountId: string
- AccountName: string
- AccountType: enum (Enterprise, Team, Individual)
- OwnerUserId: string (the User who owns this Account)
- SubscriptionTier: string
- CreatedBy: string (AdminId or System)
- Timestamp: DateTime

---

## Account.Updated
**Description**: Fired when account information is updated

**Payload**:
- AccountId: string
- UpdatedBy: string (AdminId or OwnerUserId)
- UpdatedFields: Dictionary<string, object>
- PreviousValues: Dictionary<string, object>
- Timestamp: DateTime

---

## Account.Deleted
**Description**: Fired when an account is deleted

**Payload**:
- AccountId: string
- AccountName: string
- OwnerUserId: string
- DeletedBy: string (AdminId)
- UserCount: int
- ProfileCount: int
- ContentCount: int
- Timestamp: DateTime
- DeletionType: enum (SoftDelete, HardDelete)

---

## Account.Suspended
**Description**: Fired when an account is suspended

**Payload**:
- AccountId: string
- SuspendedBy: string (AdminId)
- Reason: string
- SuspensionDuration: TimeSpan (optional)
- AffectedUserCount: int
- AffectedProfileCount: int
- Timestamp: DateTime

---

## Account.Reactivated
**Description**: Fired when a suspended account is reactivated

**Payload**:
- AccountId: string
- ReactivatedBy: string (AdminId)
- Timestamp: DateTime

---

## Account.Subscription.Changed
**Description**: Fired when account subscription tier changes

**Payload**:
- AccountId: string
- PreviousTier: string
- NewTier: string
- ChangedBy: string (OwnerUserId or AdminId)
- EffectiveDate: DateTime
- Timestamp: DateTime

---

## Account.Owner.Changed
**Description**: Fired when account ownership is transferred to a different user

**Payload**:
- AccountId: string
- PreviousOwnerUserId: string
- NewOwnerUserId: string
- TransferredBy: string (AdminId or PreviousOwnerUserId)
- Timestamp: DateTime

---

## Account.Settings.Updated
**Description**: Fired when account-level settings are modified

**Payload**:
- AccountId: string
- SettingCategory: string
- UpdatedSettings: Dictionary<string, object>
- UpdatedBy: string (AdminId or OwnerUserId)
- Timestamp: DateTime

---

## Account.Profile.Added
**Description**: Fired when a new profile is created within the account

**Payload**:
- AccountId: string
- ProfileId: string
- ProfileName: string
- CreatedBy: string (UserId)
- Timestamp: DateTime

---

## Account.Profile.Removed
**Description**: Fired when a profile is removed from the account

**Payload**:
- AccountId: string
- ProfileId: string
- ProfileName: string
- RemovedBy: string (UserId or AdminId)
- Timestamp: DateTime


