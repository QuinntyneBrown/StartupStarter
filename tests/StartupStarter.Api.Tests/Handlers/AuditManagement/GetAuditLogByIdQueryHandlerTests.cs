using FluentAssertions;
using StartupStarter.Api.Features.AuditManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.AuditManagement;

public class GetAuditLogByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAuditLog_ShouldReturnAuditLog()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var auditLog = AuditLog.Create(
            "User",
            "user-123",
            "acc-123",
            "Create",
            "admin",
            "192.168.1.1",
            new { },
            new { Name = "John Doe" }
        );
        context.AuditLogs.Add(auditLog);
        await context.SaveChangesAsync();

        var handler = new GetAuditLogByIdQueryHandler(context);
        var query = new GetAuditLogByIdQuery { AuditId = auditLog.AuditId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AuditId.Should().Be(auditLog.AuditId);
        result.EntityType.Should().Be("User");
        result.Action.Should().Be("Create");
    }

    [Fact]
    public async Task Handle_WithNonExistingAuditLog_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetAuditLogByIdQueryHandler(context);
        var query = new GetAuditLogByIdQuery { AuditId = "non-existent-audit" };

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
        var handler = new GetAuditLogByIdQueryHandler(context);
        var query = new GetAuditLogByIdQuery { AuditId = "audit-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
