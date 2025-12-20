# Role Management Models

## Core Aggregate

### RoleAggregate

Located in: `StartupStarter.Core\Model\RoleAggregate\`

#### Folder Structure
```
RoleAggregate/
├── Entities/
│   ├── Role.cs
│   └── UserRole.cs
└── Events/
    ├── RoleCreatedEvent.cs
    ├── RoleUpdatedEvent.cs
    ├── RoleDeletedEvent.cs
    ├── RoleAssignedToUserEvent.cs
    ├── RoleRevokedFromUserEvent.cs
    └── RolePermissionsUpdatedEvent.cs
```

#### Entities

Located in: `StartupStarter.Core\Model\RoleAggregate\Entities\`

**Role.cs** (Aggregate Root)
```csharp
public class Role
{
    public string RoleId { get; private set; }
    public string RoleName { get; private set; }
    public string Description { get; private set; }
    public string AccountId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private readonly List<string> _permissions = new();
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Update(Dictionary<string, object> updatedFields, string updatedBy);
    public void Delete(string deletedBy, int affectedUserCount);
    public void AssignToUser(string userId, string assignedBy);
    public void RevokeFromUser(string userId, string revokedBy, string reason);
    public void UpdatePermissions(List<string> addedPermissions, List<string> removedPermissions, string updatedBy);
}
```

**UserRole.cs**
```csharp
public class UserRole
{
    public string UserRoleId { get; private set; }
    public string RoleId { get; private set; }
    public string UserId { get; private set; }
    public string AccountId { get; private set; }
    public string AssignedBy { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokedBy { get; private set; }
    public string RevocationReason { get; private set; }
    public bool IsActive { get; private set; }

    public Role Role { get; private set; }
}
```

#### Domain Events

Located in: `StartupStarter.Core\Model\RoleAggregate\Events\`

All role events go here

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\RoleManagement\Commands\`

**CreateRoleCommand.cs**, **UpdateRoleCommand.cs**, **DeleteRoleCommand.cs**, **AssignRoleCommand.cs**, **RevokeRoleCommand.cs**, **UpdateRolePermissionsCommand.cs**
