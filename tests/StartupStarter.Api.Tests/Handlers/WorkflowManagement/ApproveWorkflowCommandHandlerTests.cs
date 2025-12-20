using FluentAssertions;
using StartupStarter.Api.Features.WorkflowManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WorkflowManagement;

public class ApproveWorkflowCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingStage_ShouldApproveWorkflow()
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

        var handler = new ApproveWorkflowCommandHandler(context);
        var command = new ApproveWorkflowCommand
        {
            StageId = "stage-123",
            ApprovedBy = "manager",
            IsApproved = true,
            Comments = "Looks good",
            ApprovalLevel = "Manager"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsApproved.Should().BeTrue();
        result.ApprovalId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithNonExistingStage_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ApproveWorkflowCommandHandler(context);
        var command = new ApproveWorkflowCommand
        {
            StageId = "non-existent-stage",
            ApprovedBy = "manager",
            IsApproved = true,
            Comments = "",
            ApprovalLevel = "Manager"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WithRejection_ShouldCreateRejectedApproval()
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

        var handler = new ApproveWorkflowCommandHandler(context);
        var command = new ApproveWorkflowCommand
        {
            StageId = "stage-456",
            ApprovedBy = "director",
            IsApproved = false,
            Comments = "Needs revision",
            ApprovalLevel = "Director"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsApproved.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ApproveWorkflowCommandHandler(context);
        var command = new ApproveWorkflowCommand
        {
            StageId = "stage-123",
            ApprovedBy = "manager",
            IsApproved = true,
            Comments = "",
            ApprovalLevel = "Manager"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
