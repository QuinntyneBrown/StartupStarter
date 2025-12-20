using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.RoleAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class RoleConfigurationTests
{
    private static StartupStarterContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StartupStarterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new StartupStarterContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void Role_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "RoleId");
    }

    [Fact]
    public void Role_RoleId_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("RoleId");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(450);
    }

    [Fact]
    public void Role_RoleName_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("RoleName");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Role_RoleName_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("RoleName");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(200);
    }

    [Fact]
    public void Role_Description_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("Description");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Role_Description_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("Description");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(500);
    }

    [Fact]
    public void Role_AccountId_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("AccountId");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Role_DomainEvents_ShouldBeIgnored()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("DomainEvents");

        // Assert
        property.Should().BeNull();
    }

    [Fact]
    public void Role_Permissions_ShouldBeIgnored()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var property = entityType!.FindProperty("Permissions");

        // Assert
        property.Should().BeNull();
    }

    [Fact]
    public void Role_ShouldHaveUserRolesRelationship()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));
        var navigation = entityType!.FindNavigation("UserRoles");

        // Assert
        navigation.Should().NotBeNull();
    }

    [Fact]
    public async Task Role_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var role = Role.Create("Admin", "Administrator role", "account-123");

        // Act
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Roles.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.RoleName.Should().Be("Admin");
    }
}
