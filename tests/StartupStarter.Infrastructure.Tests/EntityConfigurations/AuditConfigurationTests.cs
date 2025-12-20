using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.AuditAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class AuditConfigurationTests
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
    public void AuditLog_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(AuditLog));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "AuditLogId");
    }

    [Fact]
    public void AuditExport_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(AuditExport));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void RetentionPolicy_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(RetentionPolicy));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task AuditLog_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var auditLog = AuditLog.Create(
            "account-123",
            "user-123",
            "User",
            "user-456",
            AuditAction.Create,
            "Created user",
            "127.0.0.1",
            "Test Agent");

        // Act
        context.AuditLogs.Add(auditLog);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.AuditLogs.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Action.Should().Be(AuditAction.Create);
    }
}
