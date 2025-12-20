using FluentAssertions;
using StartupStarter.Api.Features.AuthenticationManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuthenticationManagement;

public class GetLoginAttemptsByUserIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAttempts_ShouldReturnAttempts()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var attempt1 = new LoginAttempt(
            "attempt-1",
            "user-123",
            "test@example.com",
            "192.168.1.1",
            "TestAgent/1.0",
            LoginMethod.Password,
            true
        );
        var attempt2 = new LoginAttempt(
            "attempt-2",
            "user-123",
            "test@example.com",
            "192.168.1.2",
            "TestAgent/2.0",
            LoginMethod.Password,
            false,
            FailureReason.InvalidCredentials
        );
        context.LoginAttempts.Add(attempt1);
        context.LoginAttempts.Add(attempt2);
        await context.SaveChangesAsync();

        var handler = new GetLoginAttemptsByUserIdQueryHandler(context);
        var query = new GetLoginAttemptsByUserIdQuery { UserId = "user-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoAttempts_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetLoginAttemptsByUserIdQueryHandler(context);
        var query = new GetLoginAttemptsByUserIdQuery { UserId = "user-with-no-attempts" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnAttemptsInDescendingOrder()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var attempt1 = new LoginAttempt(
            "attempt-1",
            "user-456",
            "test@example.com",
            "192.168.1.1",
            "OldAgent",
            LoginMethod.Password,
            true
        );
        context.LoginAttempts.Add(attempt1);
        await context.SaveChangesAsync();
        await Task.Delay(10);
        var attempt2 = new LoginAttempt(
            "attempt-2",
            "user-456",
            "test@example.com",
            "192.168.1.2",
            "NewAgent",
            LoginMethod.Password,
            true
        );
        context.LoginAttempts.Add(attempt2);
        await context.SaveChangesAsync();

        var handler = new GetLoginAttemptsByUserIdQueryHandler(context);
        var query = new GetLoginAttemptsByUserIdQuery { UserId = "user-456" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].UserAgent.Should().Be("NewAgent");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetLoginAttemptsByUserIdQueryHandler(context);
        var query = new GetLoginAttemptsByUserIdQuery { UserId = "user-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
