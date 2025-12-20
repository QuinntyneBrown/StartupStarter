using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.BackupAggregate.Entities;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;
using StartupStarter.Core.Model.SystemErrorAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class SystemConfigurationTests
{
    private static StartupStarterContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StartupStarterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new StartupStarterContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void SystemMaintenance_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(SystemMaintenance));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "MaintenanceId");
    }

    [Fact]
    public void SystemBackup_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(SystemBackup));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "BackupId");
    }

    [Fact]
    public void SystemError_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(SystemError));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "ErrorId");
    }

    [Fact]
    public async Task SystemMaintenance_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var maintenance = SystemMaintenance.Create(
            "Scheduled Maintenance",
            "System updates",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2),
            "user-123");

        // Act
        context.SystemMaintenances.Add(maintenance);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.SystemMaintenances.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Title.Should().Be("Scheduled Maintenance");
    }

    [Fact]
    public async Task SystemBackup_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var backup = SystemBackup.Create(
            BackupType.Full,
            "user-123");

        // Act
        context.SystemBackups.Add(backup);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.SystemBackups.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Type.Should().Be(BackupType.Full);
    }

    [Fact]
    public async Task SystemError_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var error = SystemError.Create(
            ErrorSeverity.Error,
            "TestComponent",
            "Test error message",
            "Stack trace here",
            null,
            null,
            null);

        // Act
        context.SystemErrors.Add(error);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.SystemErrors.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Message.Should().Be("Test error message");
    }
}
