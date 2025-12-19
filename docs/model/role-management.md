# Role Management Domain Models

This document defines the C# models needed to implement the role management events following Clean Architecture principles with MediatR.

## Domain Entities

### Role (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class Role : AggregateRoot
    {
        public Guid RoleId { get; private set; }
        public string RoleName { get; private set; }
        public string Description { get; private set; }
        public Guid AccountId { get; private set; }
        public bool IsSystemRole { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        // Navigation properties
        public virtual Account Account { get; private set; }
        public virtual User Creator { get; private set; }
        public virtual ICollection<Permission> Permissions { get; private set; } = new List<Permission>();
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        private Role() { } // EF Core

        public static Role Create(
            string roleName,
            string description,
            Guid accountId,
            Guid createdBy,
            List<string> permissions = null,
            bool isSystemRole = false)
        {
            var role = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = roleName,
                Description = description,
                AccountId = accountId,
                CreatedBy = createdBy,
                IsSystemRole = isSystemRole,
                CreatedAt = DateTime.UtcNow
            };

            if (permissions != null && permissions.Any())
            {
                foreach (var permission in permissions)
                {
                    role.Permissions.Add(Permission.Create(role.RoleId, permission));
                }
            }

            role.AddDomainEvent(new RoleCreatedDomainEvent(
                role.RoleId,
                role.RoleName,
                role.Description,
                role.AccountId,
                permissions ?? new List<string>(),
                role.CreatedBy));

            return role;
        }

        public void Update(string roleName, string description, Dictionary<string, object> updatedFields, Guid updatedBy)
        {
            var previousValues = new Dictionary<string, object>
            {
                { nameof(RoleName), RoleName },
                { nameof(Description), Description }
            };

            RoleName = roleName;
            Description = description;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new RoleUpdatedDomainEvent(
                RoleId,
                RoleName,
                AccountId,
                updatedBy,
                updatedFields,
                previousValues));
        }

        public void UpdatePermissions(
            List<string> addedPermissions,
            List<string> removedPermissions,
            Guid updatedBy)
        {
            // Remove permissions
            if (removedPermissions != null && removedPermissions.Any())
            {
                var permissionsToRemove = Permissions
                    .Where(p => removedPermissions.Contains(p.PermissionName))
                    .ToList();

                foreach (var permission in permissionsToRemove)
                {
                    Permissions.Remove(permission);
                }
            }

            // Add permissions
            if (addedPermissions != null && addedPermissions.Any())
            {
                foreach (var permissionName in addedPermissions)
                {
                    if (!Permissions.Any(p => p.PermissionName == permissionName))
                    {
                        Permissions.Add(Permission.Create(RoleId, permissionName));
                    }
                }
            }

            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new RolePermissionsUpdatedDomainEvent(
                RoleId,
                RoleName,
                AccountId,
                addedPermissions ?? new List<string>(),
                removedPermissions ?? new List<string>(),
                updatedBy));
        }

        public void AssignToUser(Guid userId, Guid assignedBy)
        {
            if (UserRoles.Any(ur => ur.UserId == userId && !ur.IsRevoked))
            {
                throw new InvalidOperationException("Role is already assigned to this user.");
            }

            var userRole = UserRole.Create(userId, RoleId, assignedBy);
            UserRoles.Add(userRole);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new RoleAssignedToUserDomainEvent(
                RoleId,
                RoleName,
                userId,
                AccountId,
                assignedBy));
        }

        public void RevokeFromUser(Guid userId, Guid revokedBy, string reason = null)
        {
            var userRole = UserRoles.FirstOrDefault(ur => ur.UserId == userId && !ur.IsRevoked);

            if (userRole == null)
            {
                throw new InvalidOperationException("Role is not assigned to this user.");
            }

            userRole.Revoke(revokedBy, reason);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new RoleRevokedFromUserDomainEvent(
                RoleId,
                RoleName,
                userId,
                AccountId,
                revokedBy,
                reason ?? string.Empty));
        }

        public void Delete(Guid deletedBy)
        {
            if (IsSystemRole)
            {
                throw new InvalidOperationException("System roles cannot be deleted.");
            }

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;

            var affectedUserCount = UserRoles.Count(ur => !ur.IsRevoked);

            AddDomainEvent(new RoleDeletedDomainEvent(
                RoleId,
                RoleName,
                AccountId,
                deletedBy,
                affectedUserCount));
        }

        public bool HasPermission(string permissionName)
        {
            return Permissions.Any(p => p.PermissionName == permissionName);
        }

        public List<string> GetAllPermissions()
        {
            return Permissions.Select(p => p.PermissionName).ToList();
        }
    }
}
```

### UserRole (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class UserRole
    {
        public Guid UserRoleId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public Guid AssignedBy { get; private set; }
        public DateTime AssignedAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public Guid? RevokedBy { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string RevokedReason { get; private set; }

        // Navigation
        public virtual User User { get; private set; }
        public virtual Role Role { get; private set; }
        public virtual User AssignedByUser { get; private set; }
        public virtual User RevokedByUser { get; private set; }

        private UserRole() { } // EF Core

        public static UserRole Create(Guid userId, Guid roleId, Guid assignedBy)
        {
            return new UserRole
            {
                UserRoleId = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsRevoked = false
            };
        }

        public void Revoke(Guid revokedBy, string reason = null)
        {
            IsRevoked = true;
            RevokedBy = revokedBy;
            RevokedAt = DateTime.UtcNow;
            RevokedReason = reason;
        }
    }
}
```

### Permission (Value Object)

```csharp
namespace StartupStarter.Domain.ValueObjects
{
    public class Permission
    {
        public Guid PermissionId { get; private set; }
        public Guid RoleId { get; private set; }
        public string PermissionName { get; private set; }
        public string Resource { get; private set; }
        public string Action { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation
        public virtual Role Role { get; private set; }

        private Permission() { } // EF Core

        public static Permission Create(Guid roleId, string permissionName)
        {
            var parts = ParsePermissionName(permissionName);

            return new Permission
            {
                PermissionId = Guid.NewGuid(),
                RoleId = roleId,
                PermissionName = permissionName,
                Resource = parts.Resource,
                Action = parts.Action,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static (string Resource, string Action) ParsePermissionName(string permissionName)
        {
            // Expected format: "Resource:Action" (e.g., "Content:Create", "User:Read")
            var parts = permissionName.Split(':');

            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid permission format: {permissionName}. Expected format: Resource:Action");
            }

            return (Resource: parts[0], Action: parts[1]);
        }

        public bool Matches(string resource, string action)
        {
            return Resource.Equals(resource, StringComparison.OrdinalIgnoreCase) &&
                   (Action == "*" || Action.Equals(action, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            if (obj is Permission other)
            {
                return PermissionName.Equals(other.PermissionName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return PermissionName.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }
}
```

## Domain Events

### RoleCreatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record RoleCreatedDomainEvent(
        Guid RoleId,
        string RoleName,
        string Description,
        Guid AccountId,
        List<string> Permissions,
        Guid CreatedBy) : DomainEvent;
}
```

### RoleUpdatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record RoleUpdatedDomainEvent(
        Guid RoleId,
        string RoleName,
        Guid AccountId,
        Guid UpdatedBy,
        Dictionary<string, object> UpdatedFields,
        Dictionary<string, object> PreviousValues) : DomainEvent;
}
```

### RoleDeletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record RoleDeletedDomainEvent(
        Guid RoleId,
        string RoleName,
        Guid AccountId,
        Guid DeletedBy,
        int AffectedUserCount) : DomainEvent;
}
```

### RoleAssignedToUserDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record RoleAssignedToUserDomainEvent(
        Guid RoleId,
        string RoleName,
        Guid UserId,
        Guid AccountId,
        Guid AssignedBy) : DomainEvent;
}
```

### RoleRevokedFromUserDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record RoleRevokedFromUserDomainEvent(
        Guid RoleId,
        string RoleName,
        Guid UserId,
        Guid AccountId,
        Guid RevokedBy,
        string Reason) : DomainEvent;
}
```

### RolePermissionsUpdatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record RolePermissionsUpdatedDomainEvent(
        Guid RoleId,
        string RoleName,
        Guid AccountId,
        List<string> AddedPermissions,
        List<string> RemovedPermissions,
        Guid UpdatedBy) : DomainEvent;
}
```

## MediatR Commands

### CreateRoleCommand

```csharp
namespace StartupStarter.Application.RoleManagement.Commands
{
    public record CreateRoleCommand(
        string RoleName,
        string Description,
        Guid AccountId,
        Guid CreatedBy,
        List<string> Permissions = null) : IRequest<CreateRoleResponse>;

    public record CreateRoleResponse(
        Guid RoleId,
        string RoleName,
        DateTime CreatedAt);
}
```

### UpdateRoleCommand

```csharp
namespace StartupStarter.Application.RoleManagement.Commands
{
    public record UpdateRoleCommand(
        Guid RoleId,
        string RoleName,
        string Description,
        Dictionary<string, object> UpdatedFields,
        Guid UpdatedBy) : IRequest<UpdateRoleResponse>;

    public record UpdateRoleResponse(
        Guid RoleId,
        DateTime UpdatedAt);
}
```

### DeleteRoleCommand

```csharp
namespace StartupStarter.Application.RoleManagement.Commands
{
    public record DeleteRoleCommand(
        Guid RoleId,
        Guid DeletedBy) : IRequest<Unit>;
}
```

### UpdateRolePermissionsCommand

```csharp
namespace StartupStarter.Application.RoleManagement.Commands
{
    public record UpdateRolePermissionsCommand(
        Guid RoleId,
        List<string> AddedPermissions,
        List<string> RemovedPermissions,
        Guid UpdatedBy) : IRequest<Unit>;
}
```

### AssignRoleToUserCommand

```csharp
namespace StartupStarter.Application.RoleManagement.Commands
{
    public record AssignRoleToUserCommand(
        Guid RoleId,
        Guid UserId,
        Guid AssignedBy) : IRequest<AssignRoleResponse>;

    public record AssignRoleResponse(
        Guid UserRoleId,
        DateTime AssignedAt);
}
```

### RevokeRoleFromUserCommand

```csharp
namespace StartupStarter.Application.RoleManagement.Commands
{
    public record RevokeRoleFromUserCommand(
        Guid RoleId,
        Guid UserId,
        Guid RevokedBy,
        string Reason = null) : IRequest<Unit>;
}
```

## MediatR Queries

### GetRoleByIdQuery

```csharp
namespace StartupStarter.Application.RoleManagement.Queries
{
    public record GetRoleByIdQuery(Guid RoleId) : IRequest<RoleDto?>;
}
```

### GetRolesByAccountQuery

```csharp
namespace StartupStarter.Application.RoleManagement.Queries
{
    public record GetRolesByAccountQuery(
        Guid AccountId,
        bool IncludeSystemRoles = false,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<RoleDto>>;
}
```

### GetUserRolesQuery

```csharp
namespace StartupStarter.Application.RoleManagement.Queries
{
    public record GetUserRolesQuery(
        Guid UserId,
        bool IncludeRevoked = false) : IRequest<List<UserRoleDto>>;
}
```

### GetRolePermissionsQuery

```csharp
namespace StartupStarter.Application.RoleManagement.Queries
{
    public record GetRolePermissionsQuery(Guid RoleId) : IRequest<List<PermissionDto>>;
}
```

### CheckUserPermissionQuery

```csharp
namespace StartupStarter.Application.RoleManagement.Queries
{
    public record CheckUserPermissionQuery(
        Guid UserId,
        string Resource,
        string Action) : IRequest<bool>;
}
```

## DTOs

### RoleDto

```csharp
namespace StartupStarter.Application.RoleManagement.DTOs
{
    public record RoleDto(
        Guid RoleId,
        string RoleName,
        string Description,
        Guid AccountId,
        bool IsSystemRole,
        int PermissionCount,
        int AssignedUserCount,
        Guid CreatedBy,
        string CreatedByName,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
```

### UserRoleDto

```csharp
namespace StartupStarter.Application.RoleManagement.DTOs
{
    public record UserRoleDto(
        Guid UserRoleId,
        Guid UserId,
        string UserName,
        Guid RoleId,
        string RoleName,
        string RoleDescription,
        Guid AssignedBy,
        string AssignedByName,
        DateTime AssignedAt,
        bool IsRevoked,
        string RevokedByName,
        DateTime? RevokedAt,
        string RevokedReason);
}
```

### PermissionDto

```csharp
namespace StartupStarter.Application.RoleManagement.DTOs
{
    public record PermissionDto(
        Guid PermissionId,
        string PermissionName,
        string Resource,
        string Action,
        DateTime CreatedAt);
}
```

## Base Classes

### AggregateRoot

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
```

### DomainEvent

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
```

## Repository Interface

### IRoleRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string roleName, Guid accountId, CancellationToken cancellationToken = default);
        Task<List<Role>> GetByAccountIdAsync(Guid accountId, bool includeSystemRoles = false, CancellationToken cancellationToken = default);
        Task<PaginatedList<Role>> GetPagedByAccountIdAsync(
            Guid accountId,
            bool includeSystemRoles,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<UserRole>> GetUserRolesAsync(Guid userId, bool includeRevoked = false, CancellationToken cancellationToken = default);
        Task<List<Permission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action, CancellationToken cancellationToken = default);
        Task<List<Guid>> GetUsersWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
        Task DeleteAsync(Role role, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

## Service Interfaces

### IRoleAuthorizationService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IRoleAuthorizationService
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
        Task<bool> UserHasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
        Task<bool> UserHasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
        Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
```
