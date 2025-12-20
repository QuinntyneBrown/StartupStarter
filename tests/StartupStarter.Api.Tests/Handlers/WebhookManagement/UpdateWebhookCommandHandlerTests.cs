using FluentAssertions;
using StartupStarter.Api.Features.WebhookManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WebhookManagement;

public class UpdateWebhookCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingWebhook_ShouldUpdateWebhook()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var webhook = new Webhook(
            "webhook-123",
            "https://old.example.com/webhook",
            "acc-123",
            "admin",
            new List<string> { "event1" }
        );
        context.Webhooks.Add(webhook);
        await context.SaveChangesAsync();

        var handler = new UpdateWebhookCommandHandler(context);
        var command = new UpdateWebhookCommand
        {
            WebhookId = "webhook-123",
            EventTypes = new List<string> { "event2", "event3" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.WebhookId.Should().Be("webhook-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingWebhook_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateWebhookCommandHandler(context);
        var command = new UpdateWebhookCommand
        {
            WebhookId = "non-existent-webhook",
            EventTypes = new List<string> { "event1" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateWebhookCommandHandler(context);
        var command = new UpdateWebhookCommand
        {
            WebhookId = "webhook-123",
            EventTypes = new List<string>()
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
