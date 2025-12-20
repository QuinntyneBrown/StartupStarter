using FluentAssertions;
using StartupStarter.Api.Features.WebhookManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WebhookManagement;

public class DeleteWebhookCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingWebhook_ShouldDeleteWebhook()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var webhook = new Webhook(
            "webhook-123",
            "https://example.com/webhook",
            "acc-123",
            "admin",
            new List<string> { "event1" }
        );
        context.Webhooks.Add(webhook);
        await context.SaveChangesAsync();

        var handler = new DeleteWebhookCommandHandler(context);
        var command = new DeleteWebhookCommand
        {
            WebhookId = "webhook-123",
            DeletedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistingWebhook_ShouldReturnFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new DeleteWebhookCommandHandler(context);
        var command = new DeleteWebhookCommand
        {
            WebhookId = "non-existent-webhook",
            DeletedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new DeleteWebhookCommandHandler(context);
        var command = new DeleteWebhookCommand
        {
            WebhookId = "webhook-123",
            DeletedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
