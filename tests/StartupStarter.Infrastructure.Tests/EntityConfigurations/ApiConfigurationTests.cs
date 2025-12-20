using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;
using StartupStarter.Core.Model.WebhookAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class ApiConfigurationTests
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
    public void ApiKey_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(ApiKey));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "ApiKeyId");
    }

    [Fact]
    public void ApiRequest_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(ApiRequest));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void Webhook_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Webhook));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "WebhookId");
    }

    [Fact]
    public void WebhookDelivery_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(WebhookDelivery));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task ApiKey_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var apiKey = ApiKey.Create(
            "Test API Key",
            "account-123",
            "user-123",
            DateTime.UtcNow.AddYears(1));

        // Act
        context.ApiKeys.Add(apiKey);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.ApiKeys.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be("Test API Key");
    }

    [Fact]
    public async Task Webhook_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var webhook = Webhook.Create(
            "Test Webhook",
            "https://example.com/webhook",
            "account-123",
            "user-123");

        // Act
        context.Webhooks.Add(webhook);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Webhooks.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be("Test Webhook");
    }
}
