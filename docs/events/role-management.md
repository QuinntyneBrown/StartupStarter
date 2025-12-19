# Role Management Events

## Role.Created
**Description**: Fired when a new role is created in the system

**Payload**:
- RoleId: string
- RoleName: string
- Description: string
- AccountId: string (account-specific role)
- Permissions: List<string>
- CreatedBy: string (AdminId or UserId)
- Timestamp: DateTime

---

## Role.Updated
**Description**: Fired when a role's properties or permissions are updated

**Payload**:
- RoleId: string
- RoleName: string
- AccountId: string
- UpdatedBy: string (AdminId or UserId)
- UpdatedFields: Dictionary<string, object>
- PreviousValues: Dictionary<string, object>
- Timestamp: DateTime

---

## Role.Deleted
**Description**: Fired when a role is deleted from the system

**Payload**:
- RoleId: string
- RoleName: string
- AccountId: string
- DeletedBy: string (AdminId or UserId)
- AffectedUserCount: int
- Timestamp: DateTime

---

## Role.AssignedToUser
**Description**: Fired when a role is assigned to a user

**Payload**:
- RoleId: string
- RoleName: string
- UserId: string
- AccountId: string
- AssignedBy: string (AdminId or UserId)
- Timestamp: DateTime

---

## Role.RevokedFromUser
**Description**: Fired when a role is revoked from a user

**Payload**:
- RoleId: string
- RoleName: string
- UserId: string
- AccountId: string
- RevokedBy: string (AdminId or UserId)
- Timestamp: DateTime
- Reason: string

---

## Role.Permissions.Updated
**Description**: Fired when permissions for a role are modified

**Payload**:
- RoleId: string
- RoleName: string
- AccountId: string
- AddedPermissions: List<string>
- RemovedPermissions: List<string>
- UpdatedBy: string (AdminId or UserId)
- Timestamp: DateTime
