using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class AccountConfigurationTests
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
    public void Account_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "AccountId");
    }

    [Fact]
    public void Account_AccountId_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("AccountId");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(450);
    }

    [Fact]
    public void Account_AccountName_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("AccountName");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Account_AccountName_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("AccountName");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(200);
    }

    [Fact]
    public void Account_OwnerUserId_ShouldBeRequired()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("OwnerUserId");

        // Assert
        property.Should().NotBeNull();
        property!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Account_SubscriptionTier_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("SubscriptionTier");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void Account_SuspensionReason_ShouldHaveMaxLength()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("SuspensionReason");

        // Assert
        property.Should().NotBeNull();
        property!.GetMaxLength().Should().Be(500);
    }

    [Fact]
    public void Account_DomainEvents_ShouldBeIgnored()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var property = entityType!.FindProperty("DomainEvents");

        // Assert
        property.Should().BeNull();
    }

    [Fact]
    public void Account_ShouldHaveSettingsRelationship()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));
        var navigation = entityType!.FindNavigation("Settings");

        // Assert
        navigation.Should().NotBeNull();
    }

    [Fact]
    public async Task Account_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var account = Account.Create("Test Account", AccountType.Personal, "owner-123", "Free");

        // Act
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Accounts.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.AccountName.Should().Be("Test Account");
    }
}
