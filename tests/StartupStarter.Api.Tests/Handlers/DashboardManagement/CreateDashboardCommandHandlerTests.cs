using FluentAssertions;
using StartupStarter.Api.Features.DashboardManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.DashboardAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.DashboardManagement;

public class CreateDashboardCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateDashboard()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateDashboardCommandHandler(context);
        var command = new CreateDashboardCommand
        {
            DashboardName = "Test Dashboard",
            ProfileId = "profile-123",
            AccountId = "acc-123",
            CreatedBy = "admin",
            IsDefault = true,
            Template = "default",
            LayoutType = LayoutType.Grid
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DashboardName.Should().Be("Test Dashboard");
        result.DashboardId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistDashboardToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateDashboardCommandHandler(context);
        var command = new CreateDashboardCommand
        {
            DashboardName = "Persistent Dashboard",
            ProfileId = "profile-456",
            AccountId = "acc-456",
            CreatedBy = "system",
            IsDefault = false,
            Template = "analytics",
            LayoutType = LayoutType.Freeform
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedDashboard = await context.Dashboards.FindAsync(result.DashboardId);
        savedDashboard.Should().NotBeNull();
        savedDashboard!.DashboardName.Should().Be("Persistent Dashboard");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueDashboardId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateDashboardCommandHandler(context);
        var command1 = new CreateDashboardCommand
        {
            DashboardName = "Dashboard One",
            ProfileId = "profile-123",
            AccountId = "acc-123",
            CreatedBy = "admin",
            IsDefault = false,
            Template = "default",
            LayoutType = LayoutType.Grid
        };
        var command2 = new CreateDashboardCommand
        {
            DashboardName = "Dashboard Two",
            ProfileId = "profile-123",
            AccountId = "acc-123",
            CreatedBy = "admin",
            IsDefault = false,
            Template = "default",
            LayoutType = LayoutType.Grid
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.DashboardId.Should().NotBe(result2.DashboardId);
    }

    [Theory]
    [InlineData(LayoutType.Grid)]
    [InlineData(LayoutType.Freeform)]
    public async Task Handle_WithDifferentLayoutTypes_ShouldCreateCorrectLayout(LayoutType layoutType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateDashboardCommandHandler(context);
        var command = new CreateDashboardCommand
        {
            DashboardName = $"Dashboard {layoutType}",
            ProfileId = "profile-123",
            AccountId = "acc-123",
            CreatedBy = "admin",
            IsDefault = false,
            Template = "default",
            LayoutType = layoutType
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.LayoutType.Should().Be(layoutType.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateDashboardCommandHandler(context);
        var command = new CreateDashboardCommand
        {
            DashboardName = "Cancel Test",
            ProfileId = "profile-123",
            AccountId = "acc-123",
            CreatedBy = "admin",
            IsDefault = false,
            Template = "default",
            LayoutType = LayoutType.Grid
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
