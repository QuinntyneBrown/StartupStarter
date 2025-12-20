using FluentAssertions;
using StartupStarter.Api.Features.WorkflowManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.WorkflowManagement;

public class StartWorkflowCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldStartWorkflow()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartWorkflowCommandHandler(context);
        var command = new StartWorkflowCommand
        {
            ContentId = "content-123",
            AccountId = "acc-123",
            WorkflowType = "Approval",
            InitiatedBy = "author"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ContentId.Should().Be("content-123");
        result.WorkflowId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistWorkflowToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartWorkflowCommandHandler(context);
        var command = new StartWorkflowCommand
        {
            ContentId = "content-456",
            AccountId = "acc-456",
            WorkflowType = "Review",
            InitiatedBy = "editor"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedWorkflow = await context.Workflows.FindAsync(result.WorkflowId);
        savedWorkflow.Should().NotBeNull();
        savedWorkflow!.ContentId.Should().Be("content-456");
        savedWorkflow.WorkflowType.Should().Be("Review");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueWorkflowId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartWorkflowCommandHandler(context);
        var command1 = new StartWorkflowCommand
        {
            ContentId = "content-1",
            AccountId = "acc-123",
            WorkflowType = "Approval",
            InitiatedBy = "user1"
        };
        var command2 = new StartWorkflowCommand
        {
            ContentId = "content-2",
            AccountId = "acc-123",
            WorkflowType = "Approval",
            InitiatedBy = "user2"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.WorkflowId.Should().NotBe(result2.WorkflowId);
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartWorkflowCommandHandler(context);
        var command = new StartWorkflowCommand
        {
            ContentId = "content-123",
            AccountId = "acc-123",
            WorkflowType = "Approval",
            InitiatedBy = "user"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
