using StartupStarter.Core.Model.AccountAggregate.Enums;
using StartupStarter.Core.Model.AccountAggregate.Events;

namespace StartupStarter.Core.Model.AccountAggregate.Entities;

public class Account
{
    public string AccountId { get; private set; }
    public string AccountName { get; private set; }
    public AccountType AccountType { get; private set; }
    public string OwnerUserId { get; private set; }
    public string SubscriptionTier { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime? SuspendedAt { get; private set; }
    public string? SuspensionReason { get; private set; }
    public TimeSpan? SuspensionDuration { get; private set; }

    private readonly List<AccountSettings> _settings = new();
    public IReadOnlyCollection<AccountSettings> Settings => _settings.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private Account() { }

    public Account(string accountId, string accountName, AccountType accountType,
        string ownerUserId, string subscriptionTier, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("AccountId cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException("AccountName cannot be empty", nameof(accountName));
        if (string.IsNullOrWhiteSpace(ownerUserId))
            throw new ArgumentException("OwnerUserId cannot be empty", nameof(ownerUserId));
        if (string.IsNullOrWhiteSpace(subscriptionTier))
            throw new ArgumentException("SubscriptionTier cannot be empty", nameof(subscriptionTier));

        AccountId = accountId;
        AccountName = accountName;
        AccountType = accountType;
        OwnerUserId = ownerUserId;
        SubscriptionTier = subscriptionTier;
        Status = AccountStatus.Active;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountCreatedEvent
        {
            AccountId = AccountId,
            AccountName = AccountName,
            AccountType = AccountType,
            OwnerUserId = OwnerUserId,
            SubscriptionTier = SubscriptionTier,
            CreatedBy = createdBy,
            Timestamp = CreatedAt
        });
    }

    public void UpdateAccountInfo(string accountName, string updatedBy)
    {
        var previousValues = new Dictionary<string, object>
        {
            { nameof(AccountName), AccountName }
        };

        AccountName = accountName;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountUpdatedEvent
        {
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = new Dictionary<string, object> { { nameof(AccountName), AccountName } },
            PreviousValues = previousValues,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ChangeSubscriptionTier(string newTier, string changedBy)
    {
        var previousTier = SubscriptionTier;
        SubscriptionTier = newTier;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountSubscriptionChangedEvent
        {
            AccountId = AccountId,
            PreviousTier = previousTier,
            NewTier = newTier,
            ChangedBy = changedBy,
            EffectiveDate = UpdatedAt.Value,
            Timestamp = UpdatedAt.Value
        });
    }

    public void TransferOwnership(string newOwnerUserId, string transferredBy)
    {
        var previousOwner = OwnerUserId;
        OwnerUserId = newOwnerUserId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountOwnerChangedEvent
        {
            AccountId = AccountId,
            PreviousOwnerUserId = previousOwner,
            NewOwnerUserId = newOwnerUserId,
            TransferredBy = transferredBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Suspend(string reason, string suspendedBy, TimeSpan? duration = null)
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException("Only active accounts can be suspended");

        Status = AccountStatus.Suspended;
        SuspendedAt = DateTime.UtcNow;
        SuspensionReason = reason;
        SuspensionDuration = duration;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountSuspendedEvent
        {
            AccountId = AccountId,
            SuspendedBy = suspendedBy,
            Reason = reason,
            SuspensionDuration = duration,
            AffectedUserCount = 0, // TODO: Calculate from related users
            AffectedProfileCount = 0, // TODO: Calculate from related profiles
            Timestamp = UpdatedAt.Value
        });
    }

    public void Reactivate(string reactivatedBy)
    {
        if (Status != AccountStatus.Suspended)
            throw new InvalidOperationException("Only suspended accounts can be reactivated");

        Status = AccountStatus.Active;
        SuspendedAt = null;
        SuspensionReason = null;
        SuspensionDuration = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountReactivatedEvent
        {
            AccountId = AccountId,
            ReactivatedBy = reactivatedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy, DeletionType deletionType)
    {
        Status = AccountStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountDeletedEvent
        {
            AccountId = AccountId,
            AccountName = AccountName,
            OwnerUserId = OwnerUserId,
            DeletedBy = deletedBy,
            UserCount = 0, // TODO: Calculate from related users
            ProfileCount = 0, // TODO: Calculate from related profiles
            ContentCount = 0, // TODO: Calculate from related content
            Timestamp = UpdatedAt.Value,
            DeletionType = deletionType
        });
    }

    public void UpdateSettings(string category, Dictionary<string, object> settings, string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AccountSettingsUpdatedEvent
        {
            AccountId = AccountId,
            SettingCategory = category,
            UpdatedSettings = settings,
            UpdatedBy = updatedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void AddProfile(string profileId, string profileName, string createdBy)
    {
        AddDomainEvent(new AccountProfileAddedEvent
        {
            AccountId = AccountId,
            ProfileId = profileId,
            ProfileName = profileName,
            CreatedBy = createdBy,
            Timestamp = DateTime.UtcNow
        });
    }

    public void RemoveProfile(string profileId, string profileName, string removedBy)
    {
        AddDomainEvent(new AccountProfileRemovedEvent
        {
            AccountId = AccountId,
            ProfileId = profileId,
            ProfileName = profileName,
            RemovedBy = removedBy,
            Timestamp = DateTime.UtcNow
        });
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
