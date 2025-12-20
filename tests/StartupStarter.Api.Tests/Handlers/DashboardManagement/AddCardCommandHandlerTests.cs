using FluentAssertions;
using StartupStarter.Api.Features.DashboardManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.DashboardManagement;

public class AddCardCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingDashboard_ShouldAddCard()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var dashboard = new Dashboard(
            "dashboard-123",
            "Test Dashboard",
            "profile-123",
            "acc-123",
            "admin",
            false,
            "default",
            LayoutType.Grid
        );
        context.Dashboards.Add(dashboard);
        await context.SaveChangesAsync();

        var handler = new AddCardCommandHandler(context);
        var command = new AddCardCommand
        {
            DashboardId = "dashboard-123",
            CardType = "Chart",
            ConfigurationJson = "{\"type\":\"bar\"}",
            Row = 0,
            Column = 0,
            Width = 2,
            Height = 2,
            AddedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DashboardId.Should().Be("dashboard-123");
        result.CardType.Should().Be("Chart");
        result.CardId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithNonExistingDashboard_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new AddCardCommandHandler(context);
        var command = new AddCardCommand
        {
            DashboardId = "non-existent-dashboard",
            CardType = "Chart",
            ConfigurationJson = "{}",
            Row = 0,
            Column = 0,
            Width = 1,
            Height = 1,
            AddedBy = "admin"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new AddCardCommandHandler(context);
        var command = new AddCardCommand
        {
            DashboardId = "dashboard-123",
            CardType = "Chart",
            ConfigurationJson = "{}",
            Row = 0,
            Column = 0,
            Width = 1,
            Height = 1,
            AddedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
