using FluentAssertions;
using StartupStarter.Api.Features.AuditManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.AuditManagement;

public class CreateAuditLogCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateAuditLog()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAuditLogCommandHandler(context);
        var command = new CreateAuditLogCommand
        {
            EntityType = "User",
            EntityId = "user-123",
            AccountId = "acc-123",
            Action = "Create",
            PerformedBy = "admin",
            IPAddress = "192.168.1.1",
            BeforeState = null,
            AfterState = new { Name = "John Doe" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EntityType.Should().Be("User");
        result.EntityId.Should().Be("user-123");
        result.Action.Should().Be("Create");
        result.AuditId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistAuditLogToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAuditLogCommandHandler(context);
        var command = new CreateAuditLogCommand
        {
            EntityType = "Account",
            EntityId = "acc-456",
            AccountId = "acc-456",
            Action = "Update",
            PerformedBy = "system",
            IPAddress = "10.0.0.1",
            BeforeState = new { Status = "Active" },
            AfterState = new { Status = "Suspended" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedLog = await context.AuditLogs.FindAsync(result.AuditId);
        savedLog.Should().NotBeNull();
        savedLog!.EntityType.Should().Be("Account");
        savedLog.Action.Should().Be("Update");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueAuditId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAuditLogCommandHandler(context);
        var command1 = new CreateAuditLogCommand
        {
            EntityType = "User",
            EntityId = "user-1",
            AccountId = "acc-123",
            Action = "Create",
            PerformedBy = "admin",
            IPAddress = "192.168.1.1"
        };
        var command2 = new CreateAuditLogCommand
        {
            EntityType = "User",
            EntityId = "user-2",
            AccountId = "acc-123",
            Action = "Create",
            PerformedBy = "admin",
            IPAddress = "192.168.1.1"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.AuditId.Should().NotBe(result2.AuditId);
    }

    [Fact]
    public async Task Handle_ShouldSetTimestampToUtcNow()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAuditLogCommandHandler(context);
        var command = new CreateAuditLogCommand
        {
            EntityType = "Content",
            EntityId = "content-123",
            AccountId = "acc-123",
            Action = "Delete",
            PerformedBy = "admin",
            IPAddress = "192.168.1.1"
        };
        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Timestamp.Should().BeOnOrAfter(beforeCreate);
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAuditLogCommandHandler(context);
        var command = new CreateAuditLogCommand
        {
            EntityType = "User",
            EntityId = "user-123",
            AccountId = "acc-123",
            Action = "Create",
            PerformedBy = "admin",
            IPAddress = "192.168.1.1"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
