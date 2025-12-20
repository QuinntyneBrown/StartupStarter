using FluentAssertions;
using StartupStarter.Core.Model.RoleAggregate.Entities;
using StartupStarter.Core.Model.RoleAggregate.Events;

namespace StartupStarter.Core.Tests.Model.RoleAggregate;

public class RoleTests
{
    private const string ValidRoleId = "role-123";
    private const string ValidRoleName = "Administrator";
    private const string ValidDescription = "Full system access";
    private const string ValidAccountId = "acc-456";
    private const string ValidCreatedBy = "admin";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidData_ShouldCreateRole()
    {
        // Arrange
        var permissions = new List<string> { "read", "write", "delete" };

        // Act
        var role = new Role(
            ValidRoleId,
            ValidRoleName,
            ValidDescription,
            ValidAccountId,
            permissions,
            ValidCreatedBy);

        // Assert
        role.RoleId.Should().Be(ValidRoleId);
        role.RoleName.Should().Be(ValidRoleName);
        role.Description.Should().Be(ValidDescription);
        role.AccountId.Should().Be(ValidAccountId);
        role.Permissions.Should().BeEquivalentTo(permissions);
        role.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        role.UpdatedAt.Should().BeNull();
        role.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldRaiseRoleCreatedEvent()
    {
        // Arrange
        var permissions = new List<string> { "read", "write" };

        // Act
        var role = new Role(
            ValidRoleId,
            ValidRoleName,
            ValidDescription,
            ValidAccountId,
            permissions,
            ValidCreatedBy);

        // Assert
        role.DomainEvents.Should().ContainSingle();
        var domainEvent = role.DomainEvents.First() as RoleCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.RoleId.Should().Be(ValidRoleId);
        domainEvent.RoleName.Should().Be(ValidRoleName);
        domainEvent.Description.Should().Be(ValidDescription);
        domainEvent.AccountId.Should().Be(ValidAccountId);
        domainEvent.Permissions.Should().BeEquivalentTo(permissions);
        domainEvent.CreatedBy.Should().Be(ValidCreatedBy);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldUseEmptyString()
    {
        // Act
        var role = new Role(
            ValidRoleId,
            ValidRoleName,
            null!,
            ValidAccountId,
            new List<string>(),
            ValidCreatedBy);

        // Assert
        role.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithEmptyPermissions_ShouldCreateRoleWithNoPermissions()
    {
        // Act
        var role = new Role(
            ValidRoleId,
            ValidRoleName,
            ValidDescription,
            ValidAccountId,
            new List<string>(),
            ValidCreatedBy);

        // Assert
        role.Permissions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNullPermissions_ShouldCreateRoleWithNoPermissions()
    {
        // Act
        var role = new Role(
            ValidRoleId,
            ValidRoleName,
            ValidDescription,
            ValidAccountId,
            null!,
            ValidCreatedBy);

        // Assert
        role.Permissions.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyRoleId_ShouldThrowArgumentException(string? roleId)
    {
        // Act & Assert
        var act = () => new Role(
            roleId!,
            ValidRoleName,
            ValidDescription,
            ValidAccountId,
            new List<string>(),
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("roleId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyRoleName_ShouldThrowArgumentException(string? roleName)
    {
        // Act & Assert
        var act = () => new Role(
            ValidRoleId,
            roleName!,
            ValidDescription,
            ValidAccountId,
            new List<string>(),
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("roleName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        // Act & Assert
        var act = () => new Role(
            ValidRoleId,
            ValidRoleName,
            ValidDescription,
            accountId!,
            new List<string>(),
            ValidCreatedBy);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("accountId");
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_WithValidData_ShouldUpdateRoleNameAndDescription()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "RoleName", "Super Admin" },
            { "Description", "Updated description" }
        };

        // Act
        role.Update(updatedFields, "admin");

        // Assert
        role.RoleName.Should().Be("Super Admin");
        role.Description.Should().Be("Updated description");
        role.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WithValidData_ShouldRaiseRoleUpdatedEvent()
    {
        // Arrange
        var role = CreateRole();
        var originalName = role.RoleName;
        role.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "RoleName", "New Role Name" }
        };

        // Act
        role.Update(updatedFields, "admin");

        // Assert
        role.DomainEvents.Should().ContainSingle();
        var domainEvent = role.DomainEvents.First() as RoleUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.RoleId.Should().Be(role.RoleId);
        domainEvent.UpdatedBy.Should().Be("admin");
        domainEvent.PreviousValues.Should().ContainKey("RoleName");
        domainEvent.PreviousValues["RoleName"].Should().Be(originalName);
    }

    [Fact]
    public void Update_WithNullUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.Update(null!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Fact]
    public void Update_WithEmptyUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.Update(new Dictionary<string, object>(), "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithEmptyUpdatedBy_ShouldThrowArgumentException(string? updatedBy)
    {
        // Arrange
        var role = CreateRole();
        var updatedFields = new Dictionary<string, object> { { "RoleName", "New Name" } };

        // Act & Assert
        var act = () => role.Update(updatedFields, updatedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedBy");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_Role_ShouldSetDeletedAt()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();

        // Act
        role.Delete("admin", 5);

        // Assert
        role.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        role.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_Role_ShouldRaiseRoleDeletedEvent()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();

        // Act
        role.Delete("admin", 10);

        // Assert
        role.DomainEvents.Should().ContainSingle();
        var domainEvent = role.DomainEvents.First() as RoleDeletedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.RoleId.Should().Be(role.RoleId);
        domainEvent.RoleName.Should().Be(role.RoleName);
        domainEvent.DeletedBy.Should().Be("admin");
        domainEvent.AffectedUserCount.Should().Be(10);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Delete_WithEmptyDeletedBy_ShouldThrowArgumentException(string? deletedBy)
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.Delete(deletedBy!, 0);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("deletedBy");
    }

    #endregion

    #region AssignToUser Tests

    [Fact]
    public void AssignToUser_WithValidData_ShouldRaiseRoleAssignedToUserEvent()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();

        // Act
        role.AssignToUser("user-123", "admin");

        // Assert
        role.DomainEvents.Should().ContainSingle();
        var domainEvent = role.DomainEvents.First() as RoleAssignedToUserEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.RoleId.Should().Be(role.RoleId);
        domainEvent.RoleName.Should().Be(role.RoleName);
        domainEvent.UserId.Should().Be("user-123");
        domainEvent.AccountId.Should().Be(role.AccountId);
        domainEvent.AssignedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AssignToUser_WithEmptyUserId_ShouldThrowArgumentException(string? userId)
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.AssignToUser(userId!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("userId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AssignToUser_WithEmptyAssignedBy_ShouldThrowArgumentException(string? assignedBy)
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.AssignToUser("user-123", assignedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("assignedBy");
    }

    #endregion

    #region RevokeFromUser Tests

    [Fact]
    public void RevokeFromUser_WithValidData_ShouldRaiseRoleRevokedFromUserEvent()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();

        // Act
        role.RevokeFromUser("user-123", "admin", "No longer needed");

        // Assert
        role.DomainEvents.Should().ContainSingle();
        var domainEvent = role.DomainEvents.First() as RoleRevokedFromUserEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.RoleId.Should().Be(role.RoleId);
        domainEvent.UserId.Should().Be("user-123");
        domainEvent.RevokedBy.Should().Be("admin");
        domainEvent.Reason.Should().Be("No longer needed");
    }

    [Fact]
    public void RevokeFromUser_WithNullReason_ShouldUseEmptyString()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();

        // Act
        role.RevokeFromUser("user-123", "admin", null!);

        // Assert
        var domainEvent = role.DomainEvents.First() as RoleRevokedFromUserEvent;
        domainEvent!.Reason.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RevokeFromUser_WithEmptyUserId_ShouldThrowArgumentException(string? userId)
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.RevokeFromUser(userId!, "admin", "reason");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("userId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RevokeFromUser_WithEmptyRevokedBy_ShouldThrowArgumentException(string? revokedBy)
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.RevokeFromUser("user-123", revokedBy!, "reason");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("revokedBy");
    }

    #endregion

    #region UpdatePermissions Tests

    [Fact]
    public void UpdatePermissions_AddingPermissions_ShouldAddToCollection()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();
        var newPermissions = new List<string> { "create", "update" };

        // Act
        role.UpdatePermissions(newPermissions, null, "admin");

        // Assert
        role.Permissions.Should().Contain("create");
        role.Permissions.Should().Contain("update");
    }

    [Fact]
    public void UpdatePermissions_RemovingPermissions_ShouldRemoveFromCollection()
    {
        // Arrange
        var initialPermissions = new List<string> { "read", "write", "delete" };
        var role = new Role(ValidRoleId, ValidRoleName, ValidDescription, ValidAccountId, initialPermissions, ValidCreatedBy);
        role.ClearDomainEvents();
        var permissionsToRemove = new List<string> { "delete" };

        // Act
        role.UpdatePermissions(null, permissionsToRemove, "admin");

        // Assert
        role.Permissions.Should().NotContain("delete");
        role.Permissions.Should().Contain("read");
        role.Permissions.Should().Contain("write");
    }

    [Fact]
    public void UpdatePermissions_WithValidData_ShouldRaiseRolePermissionsUpdatedEvent()
    {
        // Arrange
        var role = CreateRole();
        role.ClearDomainEvents();
        var addedPermissions = new List<string> { "export" };
        var removedPermissions = new List<string> { "read" };

        // Act
        role.UpdatePermissions(addedPermissions, removedPermissions, "admin");

        // Assert
        role.DomainEvents.Should().ContainSingle();
        var domainEvent = role.DomainEvents.First() as RolePermissionsUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.RoleId.Should().Be(role.RoleId);
        domainEvent.AddedPermissions.Should().BeEquivalentTo(addedPermissions);
        domainEvent.RemovedPermissions.Should().BeEquivalentTo(removedPermissions);
        domainEvent.UpdatedBy.Should().Be("admin");
    }

    [Fact]
    public void UpdatePermissions_AddingDuplicatePermission_ShouldNotAddDuplicate()
    {
        // Arrange
        var initialPermissions = new List<string> { "read" };
        var role = new Role(ValidRoleId, ValidRoleName, ValidDescription, ValidAccountId, initialPermissions, ValidCreatedBy);
        role.ClearDomainEvents();
        var duplicatePermissions = new List<string> { "read" };

        // Act
        role.UpdatePermissions(duplicatePermissions, null, "admin");

        // Assert
        role.Permissions.Count(p => p == "read").Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdatePermissions_WithEmptyUpdatedBy_ShouldThrowArgumentException(string? updatedBy)
    {
        // Arrange
        var role = CreateRole();

        // Act & Assert
        var act = () => role.UpdatePermissions(new List<string> { "test" }, null, updatedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedBy");
    }

    #endregion

    #region ClearDomainEvents Tests

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var role = CreateRole();
        role.DomainEvents.Should().NotBeEmpty();

        // Act
        role.ClearDomainEvents();

        // Assert
        role.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static Role CreateRole()
    {
        return new Role(
            ValidRoleId,
            ValidRoleName,
            ValidDescription,
            ValidAccountId,
            new List<string> { "read", "write" },
            ValidCreatedBy);
    }

    #endregion
}
