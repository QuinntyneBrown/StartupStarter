using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class AuthenticationConfigurationTests
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
    public void UserSession_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(UserSession));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "SessionId");
    }

    [Fact]
    public void LoginAttempt_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(LoginAttempt));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void MultiFactorAuthentication_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(MultiFactorAuthentication));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void PasswordResetRequest_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(PasswordResetRequest));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task UserSession_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var session = UserSession.Create(
            "user-123",
            "account-123",
            "127.0.0.1",
            "Test Agent",
            DateTime.UtcNow.AddHours(1));

        // Act
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.UserSessions.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.UserId.Should().Be("user-123");
    }

    [Fact]
    public async Task LoginAttempt_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var loginAttempt = LoginAttempt.Create(
            "test@example.com",
            true,
            "127.0.0.1",
            "Test Agent",
            null);

        // Act
        context.LoginAttempts.Add(loginAttempt);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.LoginAttempts.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Email.Should().Be("test@example.com");
    }
}
