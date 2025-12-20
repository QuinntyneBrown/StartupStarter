using FluentAssertions;
using StartupStarter.Api.Features.SystemManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;
using StartupStarter.Core.Model.MaintenanceAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.SystemManagement;

public class StartMaintenanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingMaintenance_ShouldStartMaintenance()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var maintenance = new SystemMaintenance(
            "maintenance-123",
            DateTime.UtcNow.AddMinutes(-5),
            TimeSpan.FromHours(2),
            MaintenanceType.Scheduled,
            new List<string> { "API" }
        );
        context.SystemMaintenances.Add(maintenance);
        await context.SaveChangesAsync();

        var handler = new StartMaintenanceCommandHandler(context);
        var command = new StartMaintenanceCommand
        {
            MaintenanceId = "maintenance-123"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MaintenanceId.Should().Be("maintenance-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingMaintenance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartMaintenanceCommandHandler(context);
        var command = new StartMaintenanceCommand
        {
            MaintenanceId = "non-existent-maintenance"
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
        var handler = new StartMaintenanceCommandHandler(context);
        var command = new StartMaintenanceCommand
        {
            MaintenanceId = "maintenance-123"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
