# Role Management - Detailed Design

## Overview
Role-Based Access Control (RBAC) system for managing permissions.

## Aggregates
- **RoleAggregate**: Roles with permissions and user assignments

## Key Features
- Custom roles per account
- System-wide roles (Admin, User)
- Granular permissions
- Role assignment to users
- Permission inheritance
- Audit trail for role changes

## Dependencies
- **AccountAggregate**: Roles can be account-specific
- **UserAggregate**: Users assigned to roles
- **AuditAggregate**: Role changes audited

## Business Rules
1. System roles cannot be deleted (Admin, User)
2. Custom roles scoped to account
3. User can have multiple roles
4. Permissions additive (union of all roles)
5. Deleting role affects all assigned users
6. Must specify reason when revoking role

## Data Model
**Roles Table**
- RoleId, RoleName, Description
- AccountId (null for system roles)
- Permissions (JSON array)
- CreatedAt, UpdatedAt, DeletedAt

**UserRoles Table**
- UserRoleId, RoleId, UserId
- AccountId, AssignedBy
- AssignedAt, RevokedAt
- RevokedBy, RevocationReason
- IsActive

## Permission Model
Permissions use format: `resource:action`

Examples:
- `account:read`, `account:write`, `account:delete`
- `user:create`, `user:update`, `user:deactivate`
- `content:publish`, `content:approve`
- `dashboard:create`, `dashboard:share`
- `role:assign`, `role:create`
- `*:*` (super admin)

## Predefined Roles

**System Roles:**
- **Super Admin**: All permissions (`*:*`)
- **Account Owner**: All account permissions
- **Admin**: Account management, user management
- **User**: Basic read/write permissions
- **Viewer**: Read-only access

**Custom Roles:**
- Created by Account Owners/Admins
- Scoped to account
- Flexible permission combinations

## Sequence: Assign Role
```
Admin → AssignRoleCommand
→ Validate admin has role:assign permission
→ Validate role exists
→ Validate user in same account
→ Check user doesn't already have role
→ Create UserRole record
→ Save to database
→ Publish RoleAssignedToUserEvent
→ Invalidate user permission cache
```

## API Endpoints
- POST /api/roles - Create role
- GET /api/roles - List roles
- GET /api/roles/{id} - Get role
- PUT /api/roles/{id} - Update role
- DELETE /api/roles/{id} - Delete role
- PUT /api/roles/{id}/permissions - Update permissions
- POST /api/roles/{id}/assign - Assign to user
- POST /api/roles/{id}/revoke - Revoke from user
- GET /api/roles/{id}/users - List users with role
- GET /api/users/{id}/roles - List user's roles
- GET /api/users/{id}/permissions - Get effective permissions

## Permission Checking
```csharp
public bool HasPermission(User user, string permission)
{
    var userRoles = GetActiveRoles(user.UserId);
    var allPermissions = userRoles
        .SelectMany(r => r.Permissions)
        .Distinct();

    // Check for exact match or wildcard
    return allPermissions.Any(p =>
        p == permission ||
        p == "*:*" ||
        MatchesWildcard(p, permission));
}
```

## Caching
- User permissions cached (5 min TTL)
- Cache invalidated on role changes
- Distributed cache for multi-instance

## Security
- Role changes require elevated permissions
- Audit all role assignments/revocations
- Cannot remove last admin role
- Self-service role changes prohibited
