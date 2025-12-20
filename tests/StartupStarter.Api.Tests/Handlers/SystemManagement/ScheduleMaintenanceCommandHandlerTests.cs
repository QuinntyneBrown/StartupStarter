using FluentAssertions;
using StartupStarter.Api.Features.SystemManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.MaintenanceAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.SystemManagement;

public class ScheduleMaintenanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldScheduleMaintenance()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ScheduleMaintenanceCommandHandler(context);
        var command = new ScheduleMaintenanceCommand
        {
            ScheduledStartTime = DateTime.UtcNow.AddDays(1),
            EstimatedDuration = TimeSpan.FromHours(2),
            MaintenanceType = MaintenanceType.Scheduled,
            AffectedServices = new List<string> { "API", "Database" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MaintenanceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistMaintenanceToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ScheduleMaintenanceCommandHandler(context);
        var scheduledTime = DateTime.UtcNow.AddDays(2);
        var command = new ScheduleMaintenanceCommand
        {
            ScheduledStartTime = scheduledTime,
            EstimatedDuration = TimeSpan.FromHours(4),
            MaintenanceType = MaintenanceType.Emergency,
            AffectedServices = new List<string> { "All Services" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedMaintenance = await context.SystemMaintenances.FindAsync(result.MaintenanceId);
        savedMaintenance.Should().NotBeNull();
        savedMaintenance!.ScheduledStartTime.Should().BeCloseTo(scheduledTime, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(MaintenanceType.Scheduled)]
    [InlineData(MaintenanceType.Emergency)]
    public async Task Handle_WithDifferentMaintenanceTypes_ShouldCreateCorrectType(MaintenanceType maintenanceType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ScheduleMaintenanceCommandHandler(context);
        var command = new ScheduleMaintenanceCommand
        {
            ScheduledStartTime = DateTime.UtcNow.AddDays(1),
            EstimatedDuration = TimeSpan.FromHours(1),
            MaintenanceType = maintenanceType,
            AffectedServices = new List<string> { "API" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MaintenanceType.Should().Be(maintenanceType.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ScheduleMaintenanceCommandHandler(context);
        var command = new ScheduleMaintenanceCommand
        {
            ScheduledStartTime = DateTime.UtcNow.AddDays(1),
            EstimatedDuration = TimeSpan.FromHours(1),
            MaintenanceType = MaintenanceType.Scheduled,
            AffectedServices = new List<string>()
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
