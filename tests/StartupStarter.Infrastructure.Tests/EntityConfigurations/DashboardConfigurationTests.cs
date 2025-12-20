using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class DashboardConfigurationTests
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
    public void Dashboard_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Dashboard));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "DashboardId");
    }

    [Fact]
    public void Dashboard_ShouldBeConfigured()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Dashboard));

        // Assert
        entityType.Should().NotBeNull();
    }

    [Fact]
    public void DashboardCard_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(DashboardCard));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void DashboardShare_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(DashboardShare));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task Dashboard_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var dashboard = Dashboard.Create(
            "Test Dashboard",
            "Test Description",
            "account-123",
            "user-123");

        // Act
        context.Dashboards.Add(dashboard);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Dashboards.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be("Test Dashboard");
    }
}
