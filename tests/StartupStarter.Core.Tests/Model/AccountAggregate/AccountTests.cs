using FluentAssertions;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.AccountAggregate.Enums;
using StartupStarter.Core.Model.AccountAggregate.Events;

namespace StartupStarter.Core.Tests.Model.AccountAggregate;

public class AccountTests
{
    private const string ValidAccountId = "acc-123";
    private const string ValidAccountName = "Test Account";
    private const string ValidOwnerUserId = "user-456";
    private const string ValidSubscriptionTier = "Premium";
    private const string ValidCreatedBy = "admin";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidData_ShouldCreateAccountWithActiveStatus()
    {
        // Act
        var account = new Account(
            ValidAccountId,
            ValidAccountName,
            AccountType.Enterprise,
            ValidOwnerUserId,
            ValidSubscriptionTier,
            ValidCreatedBy);

        // Assert
        account.AccountId.Should().Be(ValidAccountId);
        account.AccountName.Should().Be(ValidAccountName);
        account.AccountType.Should().Be(AccountType.Enterprise);
        account.OwnerUserId.Should().Be(ValidOwnerUserId);
        account.SubscriptionTier.Should().Be(ValidSubscriptionTier);
        account.Status.Should().Be(AccountStatus.Active);
        account.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        account.UpdatedAt.Should().BeNull();
        account.DeletedAt.Should().BeNull();
        account.SuspendedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldRaiseAccountCreatedEvent()
    {
        // Act
        var account = new Account(
            ValidAccountId,
            ValidAccountName,
            AccountType.Enterprise,
            ValidOwnerUserId,
            ValidSubscriptionTier,
            ValidCreatedBy);

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.AccountId.Should().Be(ValidAccountId);
        domainEvent.AccountName.Should().Be(ValidAccountName);
        domainEvent.AccountType.Should().Be(AccountType.Enterprise);
        domainEvent.OwnerUserId.Should().Be(ValidOwnerUserId);
        domainEvent.SubscriptionTier.Should().Be(ValidSubscriptionTier);
        domainEvent.CreatedBy.Should().Be(ValidCreatedBy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        // Act & Assert
        var act = () => new Account(
            accountId!,
            ValidAccountName,
            AccountType.Enterprise,
            ValidOwnerUserId,
            ValidSubscriptionTier,
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("accountId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAccountName_ShouldThrowArgumentException(string? accountName)
    {
        // Act & Assert
        var act = () => new Account(
            ValidAccountId,
            accountName!,
            AccountType.Enterprise,
            ValidOwnerUserId,
            ValidSubscriptionTier,
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("accountName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyOwnerUserId_ShouldThrowArgumentException(string? ownerUserId)
    {
        // Act & Assert
        var act = () => new Account(
            ValidAccountId,
            ValidAccountName,
            AccountType.Enterprise,
            ownerUserId!,
            ValidSubscriptionTier,
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("ownerUserId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptySubscriptionTier_ShouldThrowArgumentException(string? subscriptionTier)
    {
        // Act & Assert
        var act = () => new Account(
            ValidAccountId,
            ValidAccountName,
            AccountType.Enterprise,
            ValidOwnerUserId,
            subscriptionTier!,
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("subscriptionTier");
    }

    #endregion

    #region UpdateAccountInfo Tests

    [Fact]
    public void UpdateAccountInfo_WithValidData_ShouldUpdateAccountName()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();
        var newName = "Updated Account Name";

        // Act
        account.UpdateAccountInfo(newName, "admin");

        // Assert
        account.AccountName.Should().Be(newName);
        account.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateAccountInfo_WithValidData_ShouldRaiseAccountUpdatedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        var originalName = account.AccountName;
        account.ClearDomainEvents();

        // Act
        account.UpdateAccountInfo("New Name", "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.AccountId.Should().Be(account.AccountId);
        domainEvent.UpdatedBy.Should().Be("admin");
        domainEvent.PreviousValues.Should().ContainKey("AccountName");
        domainEvent.PreviousValues["AccountName"].Should().Be(originalName);
    }

    #endregion

    #region ChangeSubscriptionTier Tests

    [Fact]
    public void ChangeSubscriptionTier_WithValidData_ShouldUpdateTier()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.ChangeSubscriptionTier("Enterprise", "admin");

        // Assert
        account.SubscriptionTier.Should().Be("Enterprise");
        account.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangeSubscriptionTier_WithValidData_ShouldRaiseAccountSubscriptionChangedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        var previousTier = account.SubscriptionTier;
        account.ClearDomainEvents();

        // Act
        account.ChangeSubscriptionTier("Enterprise", "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountSubscriptionChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.PreviousTier.Should().Be(previousTier);
        domainEvent.NewTier.Should().Be("Enterprise");
        domainEvent.ChangedBy.Should().Be("admin");
    }

    #endregion

    #region TransferOwnership Tests

    [Fact]
    public void TransferOwnership_WithValidData_ShouldUpdateOwner()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();
        var newOwner = "new-owner-123";

        // Act
        account.TransferOwnership(newOwner, "admin");

        // Assert
        account.OwnerUserId.Should().Be(newOwner);
        account.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void TransferOwnership_WithValidData_ShouldRaiseAccountOwnerChangedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        var previousOwner = account.OwnerUserId;
        account.ClearDomainEvents();
        var newOwner = "new-owner-123";

        // Act
        account.TransferOwnership(newOwner, "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountOwnerChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.PreviousOwnerUserId.Should().Be(previousOwner);
        domainEvent.NewOwnerUserId.Should().Be(newOwner);
        domainEvent.TransferredBy.Should().Be("admin");
    }

    #endregion

    #region Suspend Tests

    [Fact]
    public void Suspend_ActiveAccount_ShouldChangeStatusToSuspended()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.Suspend("Policy violation", "admin", TimeSpan.FromDays(7));

        // Assert
        account.Status.Should().Be(AccountStatus.Suspended);
        account.SuspendedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        account.SuspensionReason.Should().Be("Policy violation");
        account.SuspensionDuration.Should().Be(TimeSpan.FromDays(7));
    }

    [Fact]
    public void Suspend_ActiveAccount_ShouldRaiseAccountSuspendedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.Suspend("Policy violation", "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountSuspendedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.AccountId.Should().Be(account.AccountId);
        domainEvent.SuspendedBy.Should().Be("admin");
        domainEvent.Reason.Should().Be("Policy violation");
    }

    [Fact]
    public void Suspend_NonActiveAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.Suspend("First suspension", "admin");

        // Act & Assert
        var act = () => account.Suspend("Second suspension", "admin");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only active accounts can be suspended");
    }

    #endregion

    #region Reactivate Tests

    [Fact]
    public void Reactivate_SuspendedAccount_ShouldChangeStatusToActive()
    {
        // Arrange
        var account = CreateSuspendedAccount();
        account.ClearDomainEvents();

        // Act
        account.Reactivate("admin");

        // Assert
        account.Status.Should().Be(AccountStatus.Active);
        account.SuspendedAt.Should().BeNull();
        account.SuspensionReason.Should().BeNull();
        account.SuspensionDuration.Should().BeNull();
    }

    [Fact]
    public void Reactivate_SuspendedAccount_ShouldRaiseAccountReactivatedEvent()
    {
        // Arrange
        var account = CreateSuspendedAccount();
        account.ClearDomainEvents();

        // Act
        account.Reactivate("admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountReactivatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.AccountId.Should().Be(account.AccountId);
        domainEvent.ReactivatedBy.Should().Be("admin");
    }

    [Fact]
    public void Reactivate_NonSuspendedAccount_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var account = CreateActiveAccount();

        // Act & Assert
        var act = () => account.Reactivate("admin");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only suspended accounts can be reactivated");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_Account_ShouldChangeStatusToDeleted()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.Delete("admin", DeletionType.SoftDelete);

        // Assert
        account.Status.Should().Be(AccountStatus.Deleted);
        account.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_Account_ShouldRaiseAccountDeletedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.Delete("admin", DeletionType.HardDelete);

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountDeletedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.AccountId.Should().Be(account.AccountId);
        domainEvent.DeletedBy.Should().Be("admin");
        domainEvent.DeletionType.Should().Be(DeletionType.HardDelete);
    }

    #endregion

    #region UpdateSettings Tests

    [Fact]
    public void UpdateSettings_WithValidData_ShouldRaiseAccountSettingsUpdatedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();
        var settings = new Dictionary<string, object>
        {
            { "theme", "dark" },
            { "notifications", true }
        };

        // Act
        account.UpdateSettings("appearance", settings, "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountSettingsUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.SettingCategory.Should().Be("appearance");
        domainEvent.UpdatedSettings.Should().BeEquivalentTo(settings);
    }

    #endregion

    #region AddProfile and RemoveProfile Tests

    [Fact]
    public void AddProfile_ShouldRaiseAccountProfileAddedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.AddProfile("profile-123", "Test Profile", "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountProfileAddedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ProfileId.Should().Be("profile-123");
        domainEvent.ProfileName.Should().Be("Test Profile");
    }

    [Fact]
    public void RemoveProfile_ShouldRaiseAccountProfileRemovedEvent()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.ClearDomainEvents();

        // Act
        account.RemoveProfile("profile-123", "Test Profile", "admin");

        // Assert
        account.DomainEvents.Should().ContainSingle();
        var domainEvent = account.DomainEvents.First() as AccountProfileRemovedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ProfileId.Should().Be("profile-123");
    }

    #endregion

    #region ClearDomainEvents Tests

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var account = CreateActiveAccount();
        account.DomainEvents.Should().NotBeEmpty();

        // Act
        account.ClearDomainEvents();

        // Assert
        account.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static Account CreateActiveAccount()
    {
        return new Account(
            ValidAccountId,
            ValidAccountName,
            AccountType.Enterprise,
            ValidOwnerUserId,
            ValidSubscriptionTier,
            ValidCreatedBy);
    }

    private static Account CreateSuspendedAccount()
    {
        var account = CreateActiveAccount();
        account.Suspend("Test suspension", "admin");
        return account;
    }

    #endregion
}
