using FluentAssertions;
using StartupStarter.Api.Features.WorkflowManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WorkflowManagement;

public class CompleteStageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingStage_ShouldCompleteStage()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var workflowStage = new WorkflowStage(
            "stage-123",
            "workflow-123",
            "Review",
            1,
            "reviewer"
        );
        context.WorkflowStages.Add(workflowStage);
        await context.SaveChangesAsync();

        var handler = new CompleteStageCommandHandler(context);
        var command = new CompleteStageCommand
        {
            StageId = "stage-123",
            CompletedBy = "reviewer"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.WorkflowStageId.Should().Be("stage-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingStage_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CompleteStageCommandHandler(context);
        var command = new CompleteStageCommand
        {
            StageId = "non-existent-stage",
            CompletedBy = "user"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldPersistCompletionToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var workflowStage = new WorkflowStage(
            "stage-456",
            "workflow-456",
            "Final Review",
            2,
            "reviewer"
        );
        context.WorkflowStages.Add(workflowStage);
        await context.SaveChangesAsync();

        var handler = new CompleteStageCommandHandler(context);
        var command = new CompleteStageCommand
        {
            StageId = "stage-456",
            CompletedBy = "manager"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedStage = await context.WorkflowStages.FindAsync("stage-456");
        savedStage.Should().NotBeNull();
        savedStage!.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CompleteStageCommandHandler(context);
        var command = new CompleteStageCommand
        {
            StageId = "stage-123",
            CompletedBy = "user"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
