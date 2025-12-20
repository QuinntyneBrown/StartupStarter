using FluentAssertions;
using StartupStarter.Api.Features.DashboardManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.DashboardManagement;

public class UpdateDashboardCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingDashboard_ShouldUpdateDashboard()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var dashboard = new Dashboard(
            "dashboard-123",
            "Original Name",
            "profile-123",
            "acc-123",
            "admin",
            false,
            "default",
            LayoutType.Grid
        );
        context.Dashboards.Add(dashboard);
        await context.SaveChangesAsync();

        var handler = new UpdateDashboardCommandHandler(context);
        var command = new UpdateDashboardCommand
        {
            DashboardId = "dashboard-123",
            UpdatedFields = new Dictionary<string, object> { { "DashboardName", "Updated Name" } },
            UpdatedBy = "editor"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DashboardId.Should().Be("dashboard-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingDashboard_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateDashboardCommandHandler(context);
        var command = new UpdateDashboardCommand
        {
            DashboardId = "non-existent-dashboard",
            UpdatedFields = new Dictionary<string, object>(),
            UpdatedBy = "editor"
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
        var handler = new UpdateDashboardCommandHandler(context);
        var command = new UpdateDashboardCommand
        {
            DashboardId = "dashboard-123",
            UpdatedFields = new Dictionary<string, object>(),
            UpdatedBy = "editor"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
