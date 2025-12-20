using FluentAssertions;
using StartupStarter.Api.Features.WebhookManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WebhookManagement;

public class GetWebhookByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingWebhook_ShouldReturnWebhook()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var webhook = new Webhook(
            "webhook-123",
            "https://example.com/webhook",
            "acc-123",
            "admin",
            new List<string> { "user.created", "user.updated" }
        );
        context.Webhooks.Add(webhook);
        await context.SaveChangesAsync();

        var handler = new GetWebhookByIdQueryHandler(context);
        var query = new GetWebhookByIdQuery { WebhookId = "webhook-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.WebhookId.Should().Be("webhook-123");
        result.Url.Should().Be("https://example.com/webhook");
    }

    [Fact]
    public async Task Handle_WithNonExistingWebhook_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWebhookByIdQueryHandler(context);
        var query = new GetWebhookByIdQuery { WebhookId = "non-existent-webhook" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWebhookByIdQueryHandler(context);
        var query = new GetWebhookByIdQuery { WebhookId = "webhook-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
