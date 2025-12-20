using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class UserConfigurationTests
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
    public void User_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "UserId");
    }

    [Fact]
    public void User_UserId_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("UserId");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(450);
    }

    [Fact]
    public void User_Email_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("Email");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void User_Email_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("Email");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(256);
    }

    [Fact]
    public void User_FirstName_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("FirstName");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void User_FirstName_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("FirstName");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void User_LastName_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("LastName");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void User_LastName_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("LastName");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void User_PasswordHash_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("PasswordHash");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(500);
    }

    [Fact]
    public void User_LockReason_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("LockReason");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(500);
    }

    [Fact]
    public void User_DomainEvents_ShouldBeIgnored()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("DomainEvents");

        // Assert
        property.Should().BeNull();
    }

    [Fact]
    public void User_RoleIds_ShouldBeIgnored()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var property = entityType!.FindProperty("RoleIds");

        // Assert
        property.Should().BeNull();
    }

    [Fact]
    public async Task User_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var user = User.Create(
            "test@example.com",
            "John",
            "Doe",
            "account-123",
            "hashedpassword123");

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Users.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Email.Should().Be("test@example.com");
    }
}
