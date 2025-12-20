using FluentAssertions;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Enums;
using StartupStarter.Core.Model.UserAggregate.Events;
using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Core.Tests.Model.UserAggregate;

public class UserTests
{
    private const string ValidUserId = "user-123";
    private const string ValidEmail = "test@example.com";
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidAccountId = "acc-456";
    private const string ValidPasswordHash = "hashedPassword123";
    private const string ValidCreatedBy = "admin";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidData_ShouldCreateUserWithInvitedStatus()
    {
        // Arrange
        var initialRoles = new List<string> { "role-1", "role-2" };

        // Act
        var user = new User(
            ValidUserId,
            ValidEmail,
            ValidFirstName,
            ValidLastName,
            ValidAccountId,
            ValidPasswordHash,
            initialRoles,
            ValidCreatedBy,
            true);

        // Assert
        user.UserId.Should().Be(ValidUserId);
        user.Email.Should().Be(ValidEmail);
        user.FirstName.Should().Be(ValidFirstName);
        user.LastName.Should().Be(ValidLastName);
        user.AccountId.Should().Be(ValidAccountId);
        user.PasswordHash.Should().Be(ValidPasswordHash);
        user.Status.Should().Be(UserStatus.Invited);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.RoleIds.Should().BeEquivalentTo(initialRoles);
        user.ActivatedAt.Should().BeNull();
        user.DeactivatedAt.Should().BeNull();
        user.LockedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldRaiseUserCreatedEvent()
    {
        // Arrange
        var initialRoles = new List<string> { "role-1" };

        // Act
        var user = new User(
            ValidUserId,
            ValidEmail,
            ValidFirstName,
            ValidLastName,
            ValidAccountId,
            ValidPasswordHash,
            initialRoles,
            ValidCreatedBy,
            true);

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(ValidUserId);
        domainEvent.Email.Should().Be(ValidEmail);
        domainEvent.FirstName.Should().Be(ValidFirstName);
        domainEvent.LastName.Should().Be(ValidLastName);
        domainEvent.AccountId.Should().Be(ValidAccountId);
        domainEvent.CreatedBy.Should().Be(ValidCreatedBy);
        domainEvent.InitialRoles.Should().BeEquivalentTo(initialRoles);
        domainEvent.InvitationSent.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyUserId_ShouldThrowArgumentException(string? userId)
    {
        // Act & Assert
        var act = () => new User(
            userId!,
            ValidEmail,
            ValidFirstName,
            ValidLastName,
            ValidAccountId,
            ValidPasswordHash,
            new List<string>(),
            ValidCreatedBy,
            false);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("userId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyEmail_ShouldThrowArgumentException(string? email)
    {
        // Act & Assert
        var act = () => new User(
            ValidUserId,
            email!,
            ValidFirstName,
            ValidLastName,
            ValidAccountId,
            ValidPasswordHash,
            new List<string>(),
            ValidCreatedBy,
            false);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        // Act & Assert
        var act = () => new User(
            ValidUserId,
            ValidEmail,
            ValidFirstName,
            ValidLastName,
            accountId!,
            ValidPasswordHash,
            new List<string>(),
            ValidCreatedBy,
            false);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("accountId");
    }

    #endregion

    #region Activate Tests

    [Fact]
    public void Activate_InvitedUser_ShouldChangeStatusToActive()
    {
        // Arrange
        var user = CreateInvitedUser();
        user.ClearDomainEvents();

        // Act
        user.Activate("admin", ActivationMethod.AdminActivation);

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.ActivatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_InvitedUser_ShouldRaiseUserActivatedEvent()
    {
        // Arrange
        var user = CreateInvitedUser();
        user.ClearDomainEvents();

        // Act
        user.Activate("admin", ActivationMethod.EmailVerification);

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserActivatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.UserId);
        domainEvent.ActivatedBy.Should().Be("admin");
        domainEvent.ActivationMethod.Should().Be(ActivationMethod.EmailVerification);
    }

    [Fact]
    public void Activate_InactiveUser_ShouldChangeStatusToActive()
    {
        // Arrange
        var user = CreateInactiveUser();
        user.ClearDomainEvents();

        // Act
        user.Activate("admin", ActivationMethod.AdminActivation);

        // Assert
        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public void Activate_ActiveUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = CreateActiveUser();

        // Act & Assert
        var act = () => user.Activate("admin", ActivationMethod.AdminActivation);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only invited or inactive users can be activated");
    }

    [Fact]
    public void Activate_LockedUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = CreateLockedUser();

        // Act & Assert
        var act = () => user.Activate("admin", ActivationMethod.AdminActivation);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only invited or inactive users can be activated");
    }

    #endregion

    #region Deactivate Tests

    [Fact]
    public void Deactivate_ActiveUser_ShouldChangeStatusToInactive()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Deactivate("admin", "No longer employed");

        // Assert
        user.Status.Should().Be(UserStatus.Inactive);
        user.DeactivatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_ActiveUser_ShouldRaiseUserDeactivatedEvent()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Deactivate("admin", "Voluntary leave");

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserDeactivatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.UserId);
        domainEvent.DeactivatedBy.Should().Be("admin");
        domainEvent.Reason.Should().Be("Voluntary leave");
    }

    [Fact]
    public void Deactivate_InactiveUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = CreateInactiveUser();

        // Act & Assert
        var act = () => user.Deactivate("admin", "Reason");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only active users can be deactivated");
    }

    #endregion

    #region Lock Tests

    [Fact]
    public void Lock_User_ShouldChangeStatusToLocked()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Lock("admin", "Security breach", TimeSpan.FromHours(24));

        // Assert
        user.Status.Should().Be(UserStatus.Locked);
        user.LockedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.LockReason.Should().Be("Security breach");
        user.LockDuration.Should().Be(TimeSpan.FromHours(24));
    }

    [Fact]
    public void Lock_User_ShouldRaiseUserLockedEvent()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Lock("admin", "Multiple failed login attempts", TimeSpan.FromHours(1));

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserLockedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.UserId);
        domainEvent.LockedBy.Should().Be("admin");
        domainEvent.Reason.Should().Be("Multiple failed login attempts");
        domainEvent.LockDuration.Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void Lock_WithoutDuration_ShouldLockIndefinitely()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Lock("admin", "Permanent lock");

        // Assert
        user.Status.Should().Be(UserStatus.Locked);
        user.LockDuration.Should().BeNull();
    }

    #endregion

    #region Unlock Tests

    [Fact]
    public void Unlock_LockedUser_ShouldChangeStatusToActive()
    {
        // Arrange
        var user = CreateLockedUser();
        user.ClearDomainEvents();

        // Act
        user.Unlock("admin");

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.LockedAt.Should().BeNull();
        user.LockReason.Should().BeNull();
        user.LockDuration.Should().BeNull();
    }

    [Fact]
    public void Unlock_LockedUser_ShouldRaiseUserUnlockedEvent()
    {
        // Arrange
        var user = CreateLockedUser();
        user.ClearDomainEvents();

        // Act
        user.Unlock("admin");

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserUnlockedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.UserId);
        domainEvent.UnlockedBy.Should().Be("admin");
    }

    [Fact]
    public void Unlock_NonLockedUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = CreateActiveUser();

        // Act & Assert
        var act = () => user.Unlock("admin");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only locked users can be unlocked");
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_WithValidData_ShouldUpdateFields()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "FirstName", "Jane" },
            { "LastName", "Smith" }
        };

        // Act
        user.Update(updatedFields, "admin");

        // Assert
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WithValidData_ShouldRaiseUserUpdatedEvent()
    {
        // Arrange
        var user = CreateActiveUser();
        var originalFirstName = user.FirstName;
        user.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "FirstName", "Jane" }
        };

        // Act
        user.Update(updatedFields, "admin");

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.UserId);
        domainEvent.UpdatedBy.Should().Be("admin");
        domainEvent.PreviousValues.Should().ContainKey("FirstName");
        domainEvent.PreviousValues["FirstName"].Should().Be(originalFirstName);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_User_ShouldChangeStatusToDeleted()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Delete("admin", DeletionType.SoftDelete, "User requested deletion");

        // Assert
        user.Status.Should().Be(UserStatus.Deleted);
        user.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.DeletionType.Should().Be(DeletionType.SoftDelete);
    }

    [Fact]
    public void Delete_User_ShouldRaiseUserDeletedEvent()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();

        // Act
        user.Delete("admin", DeletionType.HardDelete, "GDPR request");

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserDeletedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(user.UserId);
        domainEvent.DeletedBy.Should().Be("admin");
        domainEvent.DeletionType.Should().Be(DeletionType.HardDelete);
        domainEvent.Reason.Should().Be("GDPR request");
    }

    #endregion

    #region ChangeAccount Tests

    [Fact]
    public void ChangeAccount_WithValidData_ShouldUpdateAccountId()
    {
        // Arrange
        var user = CreateActiveUser();
        user.ClearDomainEvents();
        var newAccountId = "new-account-789";

        // Act
        user.ChangeAccount(newAccountId, "admin", "Company transfer");

        // Assert
        user.AccountId.Should().Be(newAccountId);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangeAccount_WithValidData_ShouldRaiseUserAccountChangedEvent()
    {
        // Arrange
        var user = CreateActiveUser();
        var previousAccountId = user.AccountId;
        user.ClearDomainEvents();
        var newAccountId = "new-account-789";

        // Act
        user.ChangeAccount(newAccountId, "admin", "Organizational restructure");

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First() as UserAccountChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.PreviousAccountId.Should().Be(previousAccountId);
        domainEvent.NewAccountId.Should().Be(newAccountId);
        domainEvent.ChangedBy.Should().Be("admin");
        domainEvent.Reason.Should().Be("Organizational restructure");
    }

    #endregion

    #region Role Management Tests

    [Fact]
    public void AddRole_ShouldAddRoleToCollection()
    {
        // Arrange
        var user = CreateActiveUser();
        var newRoleId = "new-role-123";

        // Act
        user.AddRole(newRoleId);

        // Assert
        user.RoleIds.Should().Contain(newRoleId);
    }

    [Fact]
    public void AddRole_DuplicateRole_ShouldNotAddDuplicate()
    {
        // Arrange
        var user = CreateActiveUser();
        var roleId = "role-123";
        user.AddRole(roleId);
        var initialCount = user.RoleIds.Count;

        // Act
        user.AddRole(roleId);

        // Assert
        user.RoleIds.Count.Should().Be(initialCount);
    }

    [Fact]
    public void RemoveRole_ExistingRole_ShouldRemoveFromCollection()
    {
        // Arrange
        var user = CreateActiveUser();
        var roleId = "role-to-remove";
        user.AddRole(roleId);

        // Act
        user.RemoveRole(roleId);

        // Assert
        user.RoleIds.Should().NotContain(roleId);
    }

    [Fact]
    public void RemoveRole_NonExistingRole_ShouldNotThrow()
    {
        // Arrange
        var user = CreateActiveUser();

        // Act & Assert
        var act = () => user.RemoveRole("non-existing-role");
        act.Should().NotThrow();
    }

    #endregion

    #region ClearDomainEvents Tests

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var user = CreateActiveUser();
        user.DomainEvents.Should().NotBeEmpty();

        // Act
        user.ClearDomainEvents();

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static User CreateInvitedUser()
    {
        return new User(
            ValidUserId,
            ValidEmail,
            ValidFirstName,
            ValidLastName,
            ValidAccountId,
            ValidPasswordHash,
            new List<string>(),
            ValidCreatedBy,
            true);
    }

    private static User CreateActiveUser()
    {
        var user = CreateInvitedUser();
        user.Activate("admin", ActivationMethod.AdminActivation);
        return user;
    }

    private static User CreateInactiveUser()
    {
        var user = CreateActiveUser();
        user.Deactivate("admin", "Reason");
        return user;
    }

    private static User CreateLockedUser()
    {
        var user = CreateActiveUser();
        user.Lock("admin", "Security reason");
        return user;
    }

    #endregion
}
