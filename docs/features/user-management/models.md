# User Management Models

## Core Aggregate

### UserAggregate

Located in: `StartupStarter.Core\Model\UserAggregate\`

#### Folder Structure
```
UserAggregate/
├── Entities/
│   ├── User.cs
│   └── UserInvitation.cs
├── Enums/
│   ├── UserStatus.cs
│   ├── DeletionType.cs
│   └── ActivationMethod.cs
└── Events/
    ├── UserCreatedEvent.cs
    ├── UserUpdatedEvent.cs
    ├── UserDeletedEvent.cs
    ├── UserActivatedEvent.cs
    ├── UserDeactivatedEvent.cs
    ├── UserLockedEvent.cs
    ├── UserUnlockedEvent.cs
    ├── UserInvitationSentEvent.cs
    ├── UserInvitationAcceptedEvent.cs
    ├── UserInvitationExpiredEvent.cs
    └── UserAccountChangedEvent.cs
```

#### Entities

Located in: `StartupStarter.Core\Model\UserAggregate\Entities\`

**User.cs** (Aggregate Root)
```csharp
public class User
{
    public string UserId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string AccountId { get; private set; }
    public string PasswordHash { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DeletionType? DeletionType { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }
    public DateTime? LockedAt { get; private set; }
    public string LockReason { get; private set; }
    public TimeSpan? LockDuration { get; private set; }

    private readonly List<string> _roleIds = new();
    public IReadOnlyCollection<string> RoleIds => _roleIds.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Update(Dictionary<string, object> updatedFields, string updatedBy);
    public void Delete(string deletedBy, DeletionType deletionType, string reason);
    public void Activate(string activatedBy, ActivationMethod method);
    public void Deactivate(string deactivatedBy, string reason);
    public void Lock(string lockedBy, string reason, TimeSpan? duration = null);
    public void Unlock(string unlockedBy);
    public void ChangeAccount(string newAccountId, string changedBy, string reason);
    public void AddRole(string roleId);
    public void RemoveRole(string roleId);
}
```

**UserInvitation.cs**
```csharp
public class UserInvitation
{
    public string InvitationId { get; private set; }
    public string Email { get; private set; }
    public string AccountId { get; private set; }
    public string InvitedBy { get; private set; }
    public DateTime SentAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public string AcceptedByUserId { get; private set; }
    public bool IsAccepted { get; private set; }
    public bool IsExpired { get; private set; }

    private readonly List<string> _roleIds = new();
    public IReadOnlyCollection<string> RoleIds => _roleIds.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Accept(string userId);
    public void MarkExpired();
}
```

#### Enums

Located in: `StartupStarter.Core\Model\UserAggregate\Enums\`

**UserStatus.cs**
```csharp
public enum UserStatus
{
    Invited,
    Active,
    Inactive,
    Locked,
    Deleted
}
```

**DeletionType.cs**
```csharp
public enum DeletionType
{
    SoftDelete,
    HardDelete
}
```

**ActivationMethod.cs**
```csharp
public enum ActivationMethod
{
    EmailVerification,
    AdminActivation,
    AutoActivation
}
```

#### Domain Events

Located in: `StartupStarter.Core\Model\UserAggregate\Events\`

All user events go here

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\UserManagement\Commands\`

**CreateUserCommand.cs**, **UpdateUserCommand.cs**, **DeleteUserCommand.cs**, **ActivateUserCommand.cs**, **DeactivateUserCommand.cs**, **LockUserCommand.cs**, **UnlockUserCommand.cs**, **SendUserInvitationCommand.cs**, **AcceptUserInvitationCommand.cs**
