using StartupStarter.Core.Model.UserAggregate.Enums;
using StartupStarter.Core.Model.UserAggregate.Events;
using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Core.Model.UserAggregate.Entities;

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
    public string? LockReason { get; private set; }
    public TimeSpan? LockDuration { get; private set; }

    private readonly List<string> _roleIds = new();
    public IReadOnlyCollection<string> RoleIds => _roleIds.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private User() { }

    public User(string userId, string email, string firstName, string lastName,
        string accountId, string passwordHash, List<string> initialRoles, string createdBy, bool invitationSent)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("AccountId cannot be empty", nameof(accountId));

        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        AccountId = accountId;
        PasswordHash = passwordHash;
        Status = UserStatus.Invited;
        CreatedAt = DateTime.UtcNow;
        _roleIds.AddRange(initialRoles);

        AddDomainEvent(new UserCreatedEvent
        {
            UserId = UserId,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            AccountId = AccountId,
            CreatedBy = createdBy,
            InitialRoles = initialRoles,
            Timestamp = CreatedAt,
            InvitationSent = invitationSent
        });
    }

    public void Update(Dictionary<string, object> updatedFields, string updatedBy)
    {
        var previousValues = new Dictionary<string, object>();

        if (updatedFields.ContainsKey(nameof(FirstName)))
        {
            previousValues[nameof(FirstName)] = FirstName;
            FirstName = updatedFields[nameof(FirstName)].ToString()!;
        }
        if (updatedFields.ContainsKey(nameof(LastName)))
        {
            previousValues[nameof(LastName)] = LastName;
            LastName = updatedFields[nameof(LastName)].ToString()!;
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUpdatedEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            PreviousValues = previousValues,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy, DeletionType deletionType, string reason)
    {
        Status = UserStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletionType = deletionType;

        AddDomainEvent(new UserDeletedEvent
        {
            UserId = UserId,
            Email = Email,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            Timestamp = UpdatedAt.Value,
            DeletionType = deletionType,
            Reason = reason
        });
    }

    public void Activate(string activatedBy, ActivationMethod method)
    {
        if (Status != UserStatus.Invited && Status != UserStatus.Inactive)
            throw new InvalidOperationException("Only invited or inactive users can be activated");

        Status = UserStatus.Active;
        ActivatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserActivatedEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            ActivatedBy = activatedBy,
            Timestamp = UpdatedAt.Value,
            ActivationMethod = method
        });
    }

    public void Deactivate(string deactivatedBy, string reason)
    {
        if (Status != UserStatus.Active)
            throw new InvalidOperationException("Only active users can be deactivated");

        Status = UserStatus.Inactive;
        DeactivatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserDeactivatedEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            DeactivatedBy = deactivatedBy,
            Timestamp = UpdatedAt.Value,
            Reason = reason
        });
    }

    public void Lock(string lockedBy, string reason, TimeSpan? duration = null)
    {
        Status = UserStatus.Locked;
        LockedAt = DateTime.UtcNow;
        LockReason = reason;
        LockDuration = duration;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserLockedEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            LockedBy = lockedBy,
            Timestamp = UpdatedAt.Value,
            Reason = reason,
            LockDuration = duration
        });
    }

    public void Unlock(string unlockedBy)
    {
        if (Status != UserStatus.Locked)
            throw new InvalidOperationException("Only locked users can be unlocked");

        Status = UserStatus.Active;
        LockedAt = null;
        LockReason = null;
        LockDuration = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUnlockedEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            UnlockedBy = unlockedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ChangeAccount(string newAccountId, string changedBy, string reason)
    {
        var previousAccountId = AccountId;
        AccountId = newAccountId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserAccountChangedEvent
        {
            UserId = UserId,
            PreviousAccountId = previousAccountId,
            NewAccountId = newAccountId,
            ChangedBy = changedBy,
            Reason = reason,
            Timestamp = UpdatedAt.Value
        });
    }

    public void AddRole(string roleId)
    {
        if (!_roleIds.Contains(roleId))
            _roleIds.Add(roleId);
    }

    public void RemoveRole(string roleId)
    {
        _roleIds.Remove(roleId);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
