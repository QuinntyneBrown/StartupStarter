using FluentAssertions;
using StartupStarter.Api.Features.WorkflowManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.WorkflowManagement;

public class GetWorkflowByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingWorkflow_ShouldReturnWorkflow()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var workflow = new Workflow(
            "workflow-123",
            "content-123",
            "acc-123",
            "Approval",
            "author"
        );
        context.Workflows.Add(workflow);
        await context.SaveChangesAsync();

        var handler = new GetWorkflowByIdQueryHandler(context);
        var query = new GetWorkflowByIdQuery { WorkflowId = "workflow-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.WorkflowId.Should().Be("workflow-123");
        result.ContentId.Should().Be("content-123");
        result.WorkflowType.Should().Be("Approval");
    }

    [Fact]
    public async Task Handle_WithNonExistingWorkflow_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetWorkflowByIdQueryHandler(context);
        var query = new GetWorkflowByIdQuery { WorkflowId = "non-existent-workflow" };

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
        var handler = new GetWorkflowByIdQueryHandler(context);
        var query = new GetWorkflowByIdQuery { WorkflowId = "workflow-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
