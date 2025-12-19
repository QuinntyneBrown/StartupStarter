# User Management Domain Models

This document defines the C# models needed to implement the user management events following Clean Architecture principles with MediatR.

## Domain Entities

### User (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class User : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Guid AccountId { get; private set; }
        public UserStatus Status { get; private set; }
        public bool IsLocked { get; private set; }
        public DateTime? LockedAt { get; private set; }
        public string? LockReason { get; private set; }
        public TimeSpan? LockDuration { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid CreatedBy { get; private set; }

        // Navigation properties
        public virtual Account Account { get; private set; }
        public virtual ICollection<UserRole> Roles { get; private set; } = new List<UserRole>();

        private User() { } // EF Core

        public static User Create(
            string email,
            string firstName,
            string lastName,
            Guid accountId,
            Guid createdBy,
            List<string> initialRoles,
            bool invitationSent = false)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                AccountId = accountId,
                Status = UserStatus.Pending,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            user.AddDomainEvent(new UserCreatedDomainEvent(
                user.UserId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.AccountId,
                createdBy,
                initialRoles,
                invitationSent));

            return user;
        }

        public void Update(Dictionary<string, object> updatedFields, Guid updatedBy)
        {
            var previousValues = new Dictionary<string, object>();

            if (updatedFields.ContainsKey("FirstName"))
            {
                previousValues["FirstName"] = FirstName;
                FirstName = updatedFields["FirstName"].ToString()!;
            }

            if (updatedFields.ContainsKey("LastName"))
            {
                previousValues["LastName"] = LastName;
                LastName = updatedFields["LastName"].ToString()!;
            }

            if (updatedFields.ContainsKey("Email"))
            {
                previousValues["Email"] = Email;
                Email = updatedFields["Email"].ToString()!;
            }

            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserUpdatedDomainEvent(
                UserId,
                AccountId,
                updatedBy,
                updatedFields,
                previousValues));
        }

        public void Activate(Guid activatedBy, ActivationMethod activationMethod)
        {
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserActivatedDomainEvent(
                UserId,
                AccountId,
                activatedBy,
                activationMethod));
        }

        public void Deactivate(Guid deactivatedBy, string reason)
        {
            Status = UserStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserDeactivatedDomainEvent(
                UserId,
                AccountId,
                deactivatedBy,
                reason));
        }

        public void Lock(string lockedBy, string reason, TimeSpan? lockDuration = null)
        {
            IsLocked = true;
            LockedAt = DateTime.UtcNow;
            LockReason = reason;
            LockDuration = lockDuration;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserLockedDomainEvent(
                UserId,
                AccountId,
                lockedBy,
                reason,
                lockDuration));
        }

        public void Unlock(Guid unlockedBy)
        {
            IsLocked = false;
            LockedAt = null;
            LockReason = null;
            LockDuration = null;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserUnlockedDomainEvent(
                UserId,
                AccountId,
                unlockedBy));
        }

        public void ChangeAccount(Guid newAccountId, Guid changedBy, string reason)
        {
            var previousAccountId = AccountId;
            AccountId = newAccountId;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserAccountChangedDomainEvent(
                UserId,
                previousAccountId,
                newAccountId,
                changedBy,
                reason));
        }

        public void Delete(Guid deletedBy, DeletionType deletionType, string reason)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserDeletedDomainEvent(
                UserId,
                Email,
                AccountId,
                deletedBy,
                deletionType,
                reason));
        }
    }
}
```

### UserInvitation (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class UserInvitation
    {
        public Guid InvitationId { get; private set; }
        public string Email { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid InvitedBy { get; private set; }
        public InvitationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? AcceptedAt { get; private set; }
        public Guid? AcceptedUserId { get; private set; }

        // Navigation properties
        public virtual Account Account { get; private set; }
        public virtual User? InvitedByUser { get; private set; }
        public virtual ICollection<InvitationRole> Roles { get; private set; } = new List<InvitationRole>();

        private UserInvitation() { } // EF Core

        public static UserInvitation Create(
            string email,
            Guid accountId,
            Guid invitedBy,
            List<string> roles,
            DateTime? expiresAt = null)
        {
            var invitation = new UserInvitation
            {
                InvitationId = Guid.NewGuid(),
                Email = email,
                AccountId = accountId,
                InvitedBy = invitedBy,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
            };

            invitation.AddDomainEvent(new UserInvitationSentDomainEvent(
                invitation.InvitationId,
                email,
                accountId,
                invitedBy,
                roles,
                invitation.ExpiresAt));

            return invitation;
        }

        public void Accept(Guid userId)
        {
            Status = InvitationStatus.Accepted;
            AcceptedAt = DateTime.UtcNow;
            AcceptedUserId = userId;

            AddDomainEvent(new UserInvitationAcceptedDomainEvent(
                InvitationId,
                userId,
                Email,
                AccountId));
        }

        public void Expire()
        {
            Status = InvitationStatus.Expired;

            AddDomainEvent(new UserInvitationExpiredDomainEvent(
                InvitationId,
                Email,
                AccountId));
        }

        private void AddDomainEvent(DomainEvent domainEvent)
        {
            // This would typically be handled through a base class or interface
            // For simplicity, assuming the pattern is similar to User aggregate
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
        public string RoleName { get; private set; }
        public DateTime AssignedAt { get; private set; }
        public Guid AssignedBy { get; private set; }

        // Navigation
        public virtual User User { get; private set; }

        private UserRole() { } // EF Core

        public static UserRole Create(Guid userId, string roleName, Guid assignedBy)
        {
            return new UserRole
            {
                UserRoleId = Guid.NewGuid(),
                UserId = userId,
                RoleName = roleName,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };
        }
    }
}
```

### InvitationRole (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class InvitationRole
    {
        public Guid InvitationRoleId { get; private set; }
        public Guid InvitationId { get; private set; }
        public string RoleName { get; private set; }

        // Navigation
        public virtual UserInvitation Invitation { get; private set; }

        private InvitationRole() { } // EF Core

        public static InvitationRole Create(Guid invitationId, string roleName)
        {
            return new InvitationRole
            {
                InvitationRoleId = Guid.NewGuid(),
                InvitationId = invitationId,
                RoleName = roleName
            };
        }
    }
}
```

## Enumerations

### UserStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum UserStatus
    {
        Pending = 0,
        Active = 1,
        Inactive = 2,
        Suspended = 3
    }
}
```

### ActivationMethod

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ActivationMethod
    {
        EmailVerification = 0,
        AdminActivation = 1,
        AutoActivation = 2
    }
}
```

### InvitationStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum InvitationStatus
    {
        Pending = 0,
        Accepted = 1,
        Expired = 2,
        Cancelled = 3
    }
}
```

### DeletionType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum DeletionType
    {
        SoftDelete = 0,
        HardDelete = 1
    }
}
```

## Domain Events

### UserCreatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserCreatedDomainEvent(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        Guid AccountId,
        Guid CreatedBy,
        List<string> InitialRoles,
        bool InvitationSent) : DomainEvent;
}
```

### UserUpdatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserUpdatedDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid UpdatedBy,
        Dictionary<string, object> UpdatedFields,
        Dictionary<string, object> PreviousValues) : DomainEvent;
}
```

### UserDeletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserDeletedDomainEvent(
        Guid UserId,
        string Email,
        Guid AccountId,
        Guid DeletedBy,
        DeletionType DeletionType,
        string Reason) : DomainEvent;
}
```

### UserActivatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserActivatedDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid ActivatedBy,
        ActivationMethod ActivationMethod) : DomainEvent;
}
```

### UserDeactivatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserDeactivatedDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid DeactivatedBy,
        string Reason) : DomainEvent;
}
```

### UserLockedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserLockedDomainEvent(
        Guid UserId,
        Guid AccountId,
        string LockedBy,
        string Reason,
        TimeSpan? LockDuration) : DomainEvent;
}
```

### UserUnlockedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserUnlockedDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid UnlockedBy) : DomainEvent;
}
```

### UserInvitationSentDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserInvitationSentDomainEvent(
        Guid InvitationId,
        string Email,
        Guid AccountId,
        Guid InvitedBy,
        List<string> Roles,
        DateTime ExpiresAt) : DomainEvent;
}
```

### UserInvitationAcceptedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserInvitationAcceptedDomainEvent(
        Guid InvitationId,
        Guid UserId,
        string Email,
        Guid AccountId) : DomainEvent;
}
```

### UserInvitationExpiredDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserInvitationExpiredDomainEvent(
        Guid InvitationId,
        string Email,
        Guid AccountId) : DomainEvent;
}
```

### UserAccountChangedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record UserAccountChangedDomainEvent(
        Guid UserId,
        Guid PreviousAccountId,
        Guid NewAccountId,
        Guid ChangedBy,
        string Reason) : DomainEvent;
}
```

## MediatR Commands

### CreateUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record CreateUserCommand(
        string Email,
        string FirstName,
        string LastName,
        Guid AccountId,
        Guid CreatedBy,
        List<string> InitialRoles,
        bool SendInvitation = false) : IRequest<CreateUserResponse>;

    public record CreateUserResponse(
        Guid UserId,
        UserStatus Status,
        DateTime CreatedAt);
}
```

### UpdateUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record UpdateUserCommand(
        Guid UserId,
        Dictionary<string, object> UpdatedFields,
        Guid UpdatedBy) : IRequest<UpdateUserResponse>;

    public record UpdateUserResponse(
        Guid UserId,
        DateTime UpdatedAt);
}
```

### DeleteUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record DeleteUserCommand(
        Guid UserId,
        Guid DeletedBy,
        DeletionType DeletionType,
        string Reason) : IRequest<Unit>;
}
```

### ActivateUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record ActivateUserCommand(
        Guid UserId,
        Guid ActivatedBy,
        ActivationMethod ActivationMethod) : IRequest<Unit>;
}
```

### DeactivateUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record DeactivateUserCommand(
        Guid UserId,
        Guid DeactivatedBy,
        string Reason) : IRequest<Unit>;
}
```

### LockUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record LockUserCommand(
        Guid UserId,
        string LockedBy,
        string Reason,
        TimeSpan? LockDuration = null) : IRequest<Unit>;
}
```

### UnlockUserCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record UnlockUserCommand(
        Guid UserId,
        Guid UnlockedBy) : IRequest<Unit>;
}
```

### SendUserInvitationCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record SendUserInvitationCommand(
        string Email,
        Guid AccountId,
        Guid InvitedBy,
        List<string> Roles,
        DateTime? ExpiresAt = null) : IRequest<SendUserInvitationResponse>;

    public record SendUserInvitationResponse(
        Guid InvitationId,
        DateTime ExpiresAt);
}
```

### AcceptUserInvitationCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record AcceptUserInvitationCommand(
        Guid InvitationId,
        string FirstName,
        string LastName,
        string Password) : IRequest<AcceptUserInvitationResponse>;

    public record AcceptUserInvitationResponse(
        Guid UserId,
        string Email,
        Guid AccountId);
}
```

### ChangeUserAccountCommand

```csharp
namespace StartupStarter.Application.Users.Commands
{
    public record ChangeUserAccountCommand(
        Guid UserId,
        Guid NewAccountId,
        Guid ChangedBy,
        string Reason) : IRequest<Unit>;
}
```

## MediatR Queries

### GetUserByIdQuery

```csharp
namespace StartupStarter.Application.Users.Queries
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;
}
```

### GetUserByEmailQuery

```csharp
namespace StartupStarter.Application.Users.Queries
{
    public record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;
}
```

### GetUsersByAccountQuery

```csharp
namespace StartupStarter.Application.Users.Queries
{
    public record GetUsersByAccountQuery(
        Guid AccountId,
        UserStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<UserDto>>;
}
```

### GetUserInvitationByIdQuery

```csharp
namespace StartupStarter.Application.Users.Queries
{
    public record GetUserInvitationByIdQuery(Guid InvitationId) : IRequest<UserInvitationDto?>;
}
```

### GetPendingInvitationsByAccountQuery

```csharp
namespace StartupStarter.Application.Users.Queries
{
    public record GetPendingInvitationsByAccountQuery(
        Guid AccountId) : IRequest<List<UserInvitationDto>>;
}
```

## DTOs

### UserDto

```csharp
namespace StartupStarter.Application.Users.DTOs
{
    public record UserDto(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        Guid AccountId,
        string AccountName,
        UserStatus Status,
        bool IsLocked,
        DateTime? LockedAt,
        string? LockReason,
        List<string> Roles,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
```

### UserInvitationDto

```csharp
namespace StartupStarter.Application.Users.DTOs
{
    public record UserInvitationDto(
        Guid InvitationId,
        string Email,
        Guid AccountId,
        string AccountName,
        string InvitedByName,
        InvitationStatus Status,
        List<string> Roles,
        DateTime CreatedAt,
        DateTime ExpiresAt,
        DateTime? AcceptedAt);
}
```

### UserSummaryDto

```csharp
namespace StartupStarter.Application.Users.DTOs
{
    public record UserSummaryDto(
        Guid UserId,
        string Email,
        string FullName,
        UserStatus Status,
        bool IsLocked);
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

## Repository Interfaces

### IUserRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<List<User>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<PaginatedList<User>> GetPagedByAccountIdAsync(
            Guid accountId,
            UserStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IUserInvitationRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IUserInvitationRepository
    {
        Task<UserInvitation?> GetByIdAsync(Guid invitationId, CancellationToken cancellationToken = default);
        Task<UserInvitation?> GetByEmailAsync(string email, Guid accountId, CancellationToken cancellationToken = default);
        Task<List<UserInvitation>> GetPendingByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<List<UserInvitation>> GetExpiredInvitationsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(UserInvitation invitation, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserInvitation invitation, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```
