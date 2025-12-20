using FluentAssertions;
using StartupStarter.Api.Features.AuthenticationManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuthenticationManagement;

public class EndSessionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSession_ShouldEndSession()
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

        var handler = new EndSessionCommandHandler(context);
        var command = new EndSessionCommand
        {
            SessionId = "session-123",
            LogoutType = LogoutType.UserInitiated
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistingSession_ShouldReturnFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new EndSessionCommandHandler(context);
        var command = new EndSessionCommand
        {
            SessionId = "non-existent-session",
            LogoutType = LogoutType.UserInitiated
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldPersistSessionEndToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var session = new UserSession(
            "session-456",
            "user-456",
            "acc-456",
            "10.0.0.1",
            "Browser/2.0",
            LoginMethod.SSO,
            DateTime.UtcNow.AddHours(2)
        );
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        var handler = new EndSessionCommandHandler(context);
        var command = new EndSessionCommand
        {
            SessionId = "session-456",
            LogoutType = LogoutType.SessionExpired
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedSession = await context.UserSessions.FindAsync("session-456");
        savedSession.Should().NotBeNull();
        savedSession!.IsActive.Should().BeFalse();
    }

    [Theory]
    [InlineData(LogoutType.UserInitiated)]
    [InlineData(LogoutType.SessionExpired)]
    [InlineData(LogoutType.AdminForced)]
    public async Task Handle_WithDifferentLogoutTypes_ShouldEndSession(LogoutType logoutType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var session = new UserSession(
            $"session-{logoutType}",
            "user-123",
            "acc-123",
            "192.168.1.1",
            "TestAgent/1.0",
            LoginMethod.Password,
            DateTime.UtcNow.AddHours(1)
        );
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        var handler = new EndSessionCommandHandler(context);
        var command = new EndSessionCommand
        {
            SessionId = $"session-{logoutType}",
            LogoutType = logoutType
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new EndSessionCommandHandler(context);
        var command = new EndSessionCommand
        {
            SessionId = "session-123",
            LogoutType = LogoutType.UserInitiated
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
