using FluentAssertions;
using StartupStarter.Api.Features.DashboardManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

namespace StartupStarter.Api.Tests.Handlers.DashboardManagement;

public class GetDashboardCardsByDashboardIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCards_ShouldReturnCards()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var position1 = new CardPosition(0, 0, 2, 2);
        var position2 = new CardPosition(0, 2, 2, 2);
        var card1 = new DashboardCard("card-1", "dashboard-123", "Chart", "{}", position1);
        var card2 = new DashboardCard("card-2", "dashboard-123", "Table", "{}", position2);
        context.DashboardCards.Add(card1);
        context.DashboardCards.Add(card2);
        await context.SaveChangesAsync();

        var handler = new GetDashboardCardsByDashboardIdQueryHandler(context);
        var query = new GetDashboardCardsByDashboardIdQuery { DashboardId = "dashboard-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoCards_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetDashboardCardsByDashboardIdQueryHandler(context);
        var query = new GetDashboardCardsByDashboardIdQuery { DashboardId = "dashboard-with-no-cards" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetDashboardCardsByDashboardIdQueryHandler(context);
        var query = new GetDashboardCardsByDashboardIdQuery { DashboardId = "dashboard-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
