using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.MediaAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class MediaConfigurationTests
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
    public void Media_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Media));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "MediaId");
    }

    [Fact]
    public void Media_ShouldBeConfigured()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Media));

        // Assert
        entityType.Should().NotBeNull();
    }

    [Fact]
    public async Task Media_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var media = Media.Create(
            "test-file.jpg",
            "test-file.jpg",
            "image/jpeg",
            1024,
            "https://example.com/files/test-file.jpg",
            "account-123",
            "user-123");

        // Act
        context.Medias.Add(media);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Medias.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.FileName.Should().Be("test-file.jpg");
    }
}
