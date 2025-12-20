using FluentAssertions;
using StartupStarter.Api.Features.SystemManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;
using StartupStarter.Core.Model.MaintenanceAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.SystemManagement;

public class GetMaintenanceByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingMaintenance_ShouldReturnMaintenance()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var maintenance = new SystemMaintenance(
            "maintenance-123",
            DateTime.UtcNow.AddDays(1),
            TimeSpan.FromHours(2),
            MaintenanceType.Scheduled,
            new List<string> { "API", "Database" }
        );
        context.SystemMaintenances.Add(maintenance);
        await context.SaveChangesAsync();

        var handler = new GetMaintenanceByIdQueryHandler(context);
        var query = new GetMaintenanceByIdQuery { MaintenanceId = "maintenance-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.MaintenanceId.Should().Be("maintenance-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingMaintenance_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetMaintenanceByIdQueryHandler(context);
        var query = new GetMaintenanceByIdQuery { MaintenanceId = "non-existent-maintenance" };

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
        var handler = new GetMaintenanceByIdQueryHandler(context);
        var query = new GetMaintenanceByIdQuery { MaintenanceId = "maintenance-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
