using FluentAssertions;
using StartupStarter.Api.Features.WorkflowManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WorkflowManagement;

public class GetWorkflowStagesByWorkflowIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingStages_ShouldReturnStages()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var stage1 = new WorkflowStage("stage-1", "workflow-123", "Review", 1, "reviewer");
        var stage2 = new WorkflowStage("stage-2", "workflow-123", "Approval", 2, "approver");
        var stage3 = new WorkflowStage("stage-3", "workflow-123", "Publish", 3, "publisher");
        context.WorkflowStages.Add(stage1);
        context.WorkflowStages.Add(stage2);
        context.WorkflowStages.Add(stage3);
        await context.SaveChangesAsync();

        var handler = new GetWorkflowStagesByWorkflowIdQueryHandler(context);
        var query = new GetWorkflowStagesByWorkflowIdQuery { WorkflowId = "workflow-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoStages_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWorkflowStagesByWorkflowIdQueryHandler(context);
        var query = new GetWorkflowStagesByWorkflowIdQuery { WorkflowId = "workflow-with-no-stages" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnStagesOrderedByStageOrder()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var stage3 = new WorkflowStage("stage-3", "workflow-456", "Publish", 3, "publisher");
        var stage1 = new WorkflowStage("stage-1", "workflow-456", "Draft", 1, "author");
        var stage2 = new WorkflowStage("stage-2", "workflow-456", "Review", 2, "reviewer");
        context.WorkflowStages.Add(stage3);
        context.WorkflowStages.Add(stage1);
        context.WorkflowStages.Add(stage2);
        await context.SaveChangesAsync();

        var handler = new GetWorkflowStagesByWorkflowIdQueryHandler(context);
        var query = new GetWorkflowStagesByWorkflowIdQuery { WorkflowId = "workflow-456" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].StageName.Should().Be("Draft");
        result[1].StageName.Should().Be("Review");
        result[2].StageName.Should().Be("Publish");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWorkflowStagesByWorkflowIdQueryHandler(context);
        var query = new GetWorkflowStagesByWorkflowIdQuery { WorkflowId = "workflow-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
