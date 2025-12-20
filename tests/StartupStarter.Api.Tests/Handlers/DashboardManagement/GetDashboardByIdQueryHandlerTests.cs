using FluentAssertions;
using StartupStarter.Api.Features.DashboardManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.DashboardManagement;

public class GetDashboardByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingDashboard_ShouldReturnDashboard()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var dashboard = new Dashboard(
            "dashboard-123",
            "Test Dashboard",
            "profile-123",
            "acc-123",
            "admin",
            true,
            "default",
            LayoutType.Grid
        );
        context.Dashboards.Add(dashboard);
        await context.SaveChangesAsync();

        var handler = new GetDashboardByIdQueryHandler(context);
        var query = new GetDashboardByIdQuery { DashboardId = "dashboard-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DashboardId.Should().Be("dashboard-123");
        result.DashboardName.Should().Be("Test Dashboard");
    }

    [Fact]
    public async Task Handle_WithNonExistingDashboard_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetDashboardByIdQueryHandler(context);
        var query = new GetDashboardByIdQuery { DashboardId = "non-existent-dashboard" };

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
        var handler = new GetDashboardByIdQueryHandler(context);
        var query = new GetDashboardByIdQuery { DashboardId = "dashboard-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
