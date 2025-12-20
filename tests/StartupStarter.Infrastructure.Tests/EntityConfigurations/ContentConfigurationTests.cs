using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.ContentAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class ContentConfigurationTests
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
    public void Content_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Content));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "ContentId");
    }

    [Fact]
    public void Content_ShouldBeConfigured()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Content));

        // Assert
        entityType.Should().NotBeNull();
    }

    [Fact]
    public void ContentVersion_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(ContentVersion));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task Content_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var content = Content.Create(
            "Test Content",
            "Test Body",
            ContentType.Article,
            "account-123",
            "user-123");

        // Act
        context.Contents.Add(content);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Contents.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Title.Should().Be("Test Content");
    }
}
