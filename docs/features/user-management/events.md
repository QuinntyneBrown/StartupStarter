# User Management Events

## User.Created
**Description**: Fired when a new user account is created (User has one Account)

**Payload**:
- UserId: string
- Email: string
- FirstName: string
- LastName: string
- AccountId: string (the single account this user belongs to)
- CreatedBy: string (UserId or AdminId or System)
- InitialRoles: List<string>
- Timestamp: DateTime
- InvitationSent: bool

---

## User.Updated
**Description**: Fired when a user's information is updated

**Payload**:
- UserId: string
- AccountId: string
- UpdatedBy: string (UserId or AdminId)
- UpdatedFields: Dictionary<string, object>
- PreviousValues: Dictionary<string, object>
- Timestamp: DateTime

---

## User.Deleted
**Description**: Fired when a user account is deleted

**Payload**:
- UserId: string
- Email: string
- AccountId: string
- DeletedBy: string (AdminId)
- Timestamp: DateTime
- DeletionType: enum (SoftDelete, HardDelete)
- Reason: string

---

## User.Activated
**Description**: Fired when a user account is activated

**Payload**:
- UserId: string
- AccountId: string
- ActivatedBy: string (UserId or AdminId)
- Timestamp: DateTime
- ActivationMethod: enum (EmailVerification, AdminActivation, AutoActivation)

---

## User.Deactivated
**Description**: Fired when a user account is deactivated

**Payload**:
- UserId: string
- AccountId: string
- DeactivatedBy: string (AdminId)
- Timestamp: DateTime
- Reason: string

---

## User.Locked
**Description**: Fired when a user account is locked (due to failed login attempts or security)

**Payload**:
- UserId: string
- AccountId: string
- LockedBy: enum (System, Admin)
- Timestamp: DateTime
- Reason: string
- LockDuration: TimeSpan (optional)

---

## User.Unlocked
**Description**: Fired when a locked user account is unlocked

**Payload**:
- UserId: string
- AccountId: string
- UnlockedBy: string (AdminId)
- Timestamp: DateTime

---

## User.Invitation.Sent
**Description**: Fired when an invitation is sent to a new user

**Payload**:
- InvitationId: string
- Email: string
- AccountId: string
- InvitedBy: string (AdminId or UserId)
- Roles: List<string>
- ExpiresAt: DateTime
- Timestamp: DateTime

---

## User.Invitation.Accepted
**Description**: Fired when a user accepts an invitation and is created

**Payload**:
- InvitationId: string
- UserId: string
- Email: string
- AccountId: string
- Timestamp: DateTime

---

## User.Invitation.Expired
**Description**: Fired when an invitation expires without being accepted

**Payload**:
- InvitationId: string
- Email: string
- AccountId: string
- Timestamp: DateTime

---

## User.Account.Changed
**Description**: Fired when a user is moved to a different account (rare administrative action)

**Payload**:
- UserId: string
- PreviousAccountId: string
- NewAccountId: string
- ChangedBy: string (AdminId)
- Reason: string
- Timestamp: DateTime
