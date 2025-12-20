using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.EntityConfigurations;

public class WorkflowConfigurationTests
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
    public void Workflow_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Workflow));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "WorkflowId");
    }

    [Fact]
    public void WorkflowStage_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(WorkflowStage));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public void WorkflowApproval_ShouldHavePrimaryKey()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(WorkflowApproval));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
    }

    [Fact]
    public async Task Workflow_CanBePersisted()
    {
        // Arrange
        using var context = CreateContext();
        var workflow = Workflow.Create(
            "Test Workflow",
            "Test Description",
            "account-123",
            "user-123");

        // Act
        context.Workflows.Add(workflow);
        await context.SaveChangesAsync();

        // Assert
        var persisted = await context.Workflows.FirstOrDefaultAsync();
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be("Test Workflow");
    }
}
