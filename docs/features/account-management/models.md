# Account Management Models

## Core Aggregate

### AccountAggregate

Located in: `StartupStarter.Core\Model\AccountAggregate\`

#### Folder Structure
```
AccountAggregate/
├── Entities/
│   ├── Account.cs
│   └── AccountSettings.cs
├── Enums/
│   ├── AccountType.cs
│   ├── AccountStatus.cs
│   └── DeletionType.cs
└── Events/
    ├── AccountCreatedEvent.cs
    ├── AccountUpdatedEvent.cs
    ├── AccountDeletedEvent.cs
    ├── AccountSuspendedEvent.cs
    ├── AccountReactivatedEvent.cs
    ├── AccountSubscriptionChangedEvent.cs
    ├── AccountOwnerChangedEvent.cs
    ├── AccountSettingsUpdatedEvent.cs
    ├── AccountProfileAddedEvent.cs
    └── AccountProfileRemovedEvent.cs
```

#### Entities

Located in: `StartupStarter.Core\Model\AccountAggregate\Entities\`

**Account.cs** (Aggregate Root)
```csharp
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
    public string SuspensionReason { get; private set; }
    public TimeSpan? SuspensionDuration { get; private set; }

    private readonly List<AccountSettings> _settings = new();
    public IReadOnlyCollection<AccountSettings> Settings => _settings.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Methods
    public void UpdateAccountInfo(string accountName, string updatedBy);
    public void ChangeSubscriptionTier(string newTier, string changedBy);
    public void TransferOwnership(string newOwnerUserId, string transferredBy);
    public void Suspend(string reason, string suspendedBy, TimeSpan? duration = null);
    public void Reactivate(string reactivatedBy);
    public void Delete(string deletedBy, DeletionType deletionType);
    public void UpdateSettings(string category, Dictionary<string, object> settings, string updatedBy);
    public void AddProfile(string profileId, string profileName, string createdBy);
    public void RemoveProfile(string profileId, string profileName, string removedBy);
}
```

**AccountSettings.cs**
```csharp
public class AccountSettings
{
    public string AccountSettingsId { get; private set; }
    public string AccountId { get; private set; }
    public string Category { get; private set; }
    public string SettingsJson { get; private set; } // Stores JSON serialized settings
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Account Account { get; private set; }
}
```

#### Enums

Located in: `StartupStarter.Core\Model\AccountAggregate\Enums\`

**AccountType.cs**
```csharp
public enum AccountType
{
    Individual,
    Team,
    Enterprise
}
```

**AccountStatus.cs**
```csharp
public enum AccountStatus
{
    Active,
    Suspended,
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

#### Domain Events

Located in: `StartupStarter.Core\Model\AccountAggregate\Events\`

**AccountCreatedEvent.cs**
```csharp
public class AccountCreatedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string AccountName { get; set; }
    public AccountType AccountType { get; set; }
    public string OwnerUserId { get; set; }
    public string SubscriptionTier { get; set; }
    public string CreatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountUpdatedEvent.cs**
```csharp
public class AccountUpdatedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string UpdatedBy { get; set; }
    public Dictionary<string, object> UpdatedFields { get; set; }
    public Dictionary<string, object> PreviousValues { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountDeletedEvent.cs**
```csharp
public class AccountDeletedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string AccountName { get; set; }
    public string OwnerUserId { get; set; }
    public string DeletedBy { get; set; }
    public int UserCount { get; set; }
    public int ProfileCount { get; set; }
    public int ContentCount { get; set; }
    public DateTime Timestamp { get; set; }
    public DeletionType DeletionType { get; set; }
}
```

**AccountSuspendedEvent.cs**
```csharp
public class AccountSuspendedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string SuspendedBy { get; set; }
    public string Reason { get; set; }
    public TimeSpan? SuspensionDuration { get; set; }
    public int AffectedUserCount { get; set; }
    public int AffectedProfileCount { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountReactivatedEvent.cs**
```csharp
public class AccountReactivatedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string ReactivatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountSubscriptionChangedEvent.cs**
```csharp
public class AccountSubscriptionChangedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string PreviousTier { get; set; }
    public string NewTier { get; set; }
    public string ChangedBy { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountOwnerChangedEvent.cs**
```csharp
public class AccountOwnerChangedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string PreviousOwnerUserId { get; set; }
    public string NewOwnerUserId { get; set; }
    public string TransferredBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountSettingsUpdatedEvent.cs**
```csharp
public class AccountSettingsUpdatedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string SettingCategory { get; set; }
    public Dictionary<string, object> UpdatedSettings { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountProfileAddedEvent.cs**
```csharp
public class AccountProfileAddedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string ProfileId { get; set; }
    public string ProfileName { get; set; }
    public string CreatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AccountProfileRemovedEvent.cs**
```csharp
public class AccountProfileRemovedEvent : DomainEvent
{
    public string AccountId { get; set; }
    public string ProfileId { get; set; }
    public string ProfileName { get; set; }
    public string RemovedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Infrastructure

### Entity Configuration

**AccountConfiguration.cs**
Located in: `StartupStarter.Infrastructure\EntityConfigurations\`

```csharp
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.AccountId);

        builder.Property(a => a.AccountId).IsRequired();
        builder.Property(a => a.AccountName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.OwnerUserId).IsRequired();
        builder.Property(a => a.SubscriptionTier).IsRequired().HasMaxLength(50);

        builder.HasMany(a => a.Settings)
            .WithOne(s => s.Account)
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.DomainEvents);
    }
}
```

**AccountSettingsConfiguration.cs**
```csharp
public class AccountSettingsConfiguration : IEntityTypeConfiguration<AccountSettings>
{
    public void Configure(EntityTypeBuilder<AccountSettings> builder)
    {
        builder.ToTable("AccountSettings");
        builder.HasKey(s => s.AccountSettingsId);

        builder.Property(s => s.SettingsJson).IsRequired();
        builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
    }
}
```

## API Layer

### DTOs

**AccountDto.cs**
Located in: `StartupStarter.Api\Dtos\`

```csharp
public class AccountDto
{
    public string AccountId { get; set; }
    public string AccountName { get; set; }
    public string AccountType { get; set; }
    public string OwnerUserId { get; set; }
    public string SubscriptionTier { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**CreateAccountDto.cs**
```csharp
public class CreateAccountDto
{
    public string AccountName { get; set; }
    public AccountType AccountType { get; set; }
    public string OwnerUserId { get; set; }
    public string SubscriptionTier { get; set; }
}
```

**UpdateAccountDto.cs**
```csharp
public class UpdateAccountDto
{
    public string AccountName { get; set; }
    public Dictionary<string, object> UpdatedFields { get; set; }
}
```

### Extension Methods

**AccountExtensions.cs**
Located in: `StartupStarter.Api\Extensions\`

```csharp
public static class AccountExtensions
{
    public static AccountDto ToDto(this Account account)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountType = account.AccountType.ToString(),
            OwnerUserId = account.OwnerUserId,
            SubscriptionTier = account.SubscriptionTier,
            Status = account.Status.ToString(),
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }
}
```

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\AccountManagement\Commands\`

**CreateAccountCommand.cs**
```csharp
public class CreateAccountCommand : IRequest<AccountDto>
{
    public string AccountName { get; set; }
    public AccountType AccountType { get; set; }
    public string OwnerUserId { get; set; }
    public string SubscriptionTier { get; set; }
    public string CreatedBy { get; set; }
}
```

**UpdateAccountCommand.cs**
```csharp
public class UpdateAccountCommand : IRequest<AccountDto>
{
    public string AccountId { get; set; }
    public Dictionary<string, object> UpdatedFields { get; set; }
    public string UpdatedBy { get; set; }
}
```

**DeleteAccountCommand.cs**
```csharp
public class DeleteAccountCommand : IRequest<bool>
{
    public string AccountId { get; set; }
    public string DeletedBy { get; set; }
    public DeletionType DeletionType { get; set; }
}
```

**SuspendAccountCommand.cs**
```csharp
public class SuspendAccountCommand : IRequest<bool>
{
    public string AccountId { get; set; }
    public string Reason { get; set; }
    public string SuspendedBy { get; set; }
    public TimeSpan? SuspensionDuration { get; set; }
}
```

**ReactivateAccountCommand.cs**
```csharp
public class ReactivateAccountCommand : IRequest<bool>
{
    public string AccountId { get; set; }
    public string ReactivatedBy { get; set; }
}
```

**ChangeSubscriptionTierCommand.cs**
```csharp
public class ChangeSubscriptionTierCommand : IRequest<bool>
{
    public string AccountId { get; set; }
    public string NewTier { get; set; }
    public string ChangedBy { get; set; }
}
```

**TransferAccountOwnershipCommand.cs**
```csharp
public class TransferAccountOwnershipCommand : IRequest<bool>
{
    public string AccountId { get; set; }
    public string NewOwnerUserId { get; set; }
    public string TransferredBy { get; set; }
}
```

**UpdateAccountSettingsCommand.cs**
```csharp
public class UpdateAccountSettingsCommand : IRequest<bool>
{
    public string AccountId { get; set; }
    public string SettingCategory { get; set; }
    public Dictionary<string, object> UpdatedSettings { get; set; }
    public string UpdatedBy { get; set; }
}
```

### Queries (MediatR)

Located in: `StartupStarter.Api\Features\AccountManagement\Queries\`

**GetAccountByIdQuery.cs**
```csharp
public class GetAccountByIdQuery : IRequest<AccountDto>
{
    public string AccountId { get; set; }
}
```

**GetAccountsByOwnerQuery.cs**
```csharp
public class GetAccountsByOwnerQuery : IRequest<List<AccountDto>>
{
    public string OwnerUserId { get; set; }
}
```

**GetAccountsBySubscriptionTierQuery.cs**
```csharp
public class GetAccountsBySubscriptionTierQuery : IRequest<List<AccountDto>>
{
    public string SubscriptionTier { get; set; }
}
```
