using FluentAssertions;
using StartupStarter.Api.Features.AuthenticationManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuthenticationManagement;

public class GetSessionByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSession_ShouldReturnSession()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var session = new UserSession(
            "session-123",
            "user-123",
            "acc-123",
            "192.168.1.1",
            "TestAgent/1.0",
            LoginMethod.Password,
            DateTime.UtcNow.AddHours(1)
        );
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        var handler = new GetSessionByIdQueryHandler(context);
        var query = new GetSessionByIdQuery { SessionId = "session-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.SessionId.Should().Be("session-123");
        result.UserId.Should().Be("user-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingSession_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetSessionByIdQueryHandler(context);
        var query = new GetSessionByIdQuery { SessionId = "non-existent-session" };

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
        var handler = new GetSessionByIdQueryHandler(context);
        var query = new GetSessionByIdQuery { SessionId = "session-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
