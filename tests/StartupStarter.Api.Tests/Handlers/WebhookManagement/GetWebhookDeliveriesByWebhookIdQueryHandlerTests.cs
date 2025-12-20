using FluentAssertions;
using StartupStarter.Api.Features.WebhookManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WebhookManagement;

public class GetWebhookDeliveriesByWebhookIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingDeliveries_ShouldReturnDeliveries()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var delivery1 = new WebhookDelivery(
            "delivery-1",
            "webhook-123",
            "user.created",
            "{\"userId\":\"123\"}",
            200,
            "Success"
        );
        var delivery2 = new WebhookDelivery(
            "delivery-2",
            "webhook-123",
            "user.updated",
            "{\"userId\":\"456\"}",
            500,
            "Server Error"
        );
        context.WebhookDeliveries.Add(delivery1);
        context.WebhookDeliveries.Add(delivery2);
        await context.SaveChangesAsync();

        var handler = new GetWebhookDeliveriesByWebhookIdQueryHandler(context);
        var query = new GetWebhookDeliveriesByWebhookIdQuery { WebhookId = "webhook-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoDeliveries_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWebhookDeliveriesByWebhookIdQueryHandler(context);
        var query = new GetWebhookDeliveriesByWebhookIdQuery { WebhookId = "webhook-with-no-deliveries" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnDeliveriesInDescendingOrder()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var delivery1 = new WebhookDelivery(
            "delivery-1",
            "webhook-456",
            "old.event",
            "{}",
            200,
            "OK"
        );
        context.WebhookDeliveries.Add(delivery1);
        await context.SaveChangesAsync();
        await Task.Delay(10);
        var delivery2 = new WebhookDelivery(
            "delivery-2",
            "webhook-456",
            "new.event",
            "{}",
            200,
            "OK"
        );
        context.WebhookDeliveries.Add(delivery2);
        await context.SaveChangesAsync();

        var handler = new GetWebhookDeliveriesByWebhookIdQueryHandler(context);
        var query = new GetWebhookDeliveriesByWebhookIdQuery { WebhookId = "webhook-456" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].EventType.Should().Be("new.event");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWebhookDeliveriesByWebhookIdQueryHandler(context);
        var query = new GetWebhookDeliveriesByWebhookIdQuery { WebhookId = "webhook-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
