using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.ProfileAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class ProfileConfigurationTests
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
    public void Profile_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Profile));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "ProfileId");
    }

    [Fact]
    public void ProfilePreferences_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(ProfilePreferences));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void ProfileShare_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(ProfileShare));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task Profile_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var profile = Profile.Create(
            "user-123",
            "Test Bio",
            "New York",
            "https://example.com",
            "https://example.com/avatar.jpg");

        // Act
        context.Profiles.Add(profile);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Profiles.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Bio.Should().Be("Test Bio");
    }
}
