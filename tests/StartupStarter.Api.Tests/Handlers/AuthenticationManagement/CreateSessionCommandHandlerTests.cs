using FluentAssertions;
using StartupStarter.Api.Features.AuthenticationManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuthenticationManagement;

public class CreateSessionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateSession()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateSessionCommandHandler(context);
        var command = new CreateSessionCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0",
            LoginMethod = LoginMethod.Password,
            ExpirationMinutes = 60
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-123");
        result.AccountId.Should().Be("acc-123");
        result.SessionId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistSessionToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateSessionCommandHandler(context);
        var command = new CreateSessionCommand
        {
            UserId = "user-456",
            AccountId = "acc-456",
            IpAddress = "10.0.0.1",
            UserAgent = "Browser/2.0",
            LoginMethod = LoginMethod.SSO,
            ExpirationMinutes = 120
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedSession = await context.UserSessions.FindAsync(result.SessionId);
        savedSession.Should().NotBeNull();
        savedSession!.UserId.Should().Be("user-456");
        savedSession.IpAddress.Should().Be("10.0.0.1");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueSessionId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateSessionCommandHandler(context);
        var command1 = new CreateSessionCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0",
            LoginMethod = LoginMethod.Password
        };
        var command2 = new CreateSessionCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            IpAddress = "192.168.1.2",
            UserAgent = "TestAgent/2.0",
            LoginMethod = LoginMethod.Password
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.SessionId.Should().NotBe(result2.SessionId);
    }

    [Fact]
    public async Task Handle_WithExpirationMinutes_ShouldSetExpirationTime()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateSessionCommandHandler(context);
        var beforeCreate = DateTime.UtcNow;
        var command = new CreateSessionCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0",
            LoginMethod = LoginMethod.Password,
            ExpirationMinutes = 60
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ExpiresAt.Should().NotBeNull();
        result.ExpiresAt!.Value.Should().BeCloseTo(beforeCreate.AddMinutes(60), TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(LoginMethod.Password)]
    [InlineData(LoginMethod.SSO)]
    [InlineData(LoginMethod.ApiKey)]
    public async Task Handle_WithDifferentLoginMethods_ShouldCreateCorrectSession(LoginMethod method)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateSessionCommandHandler(context);
        var command = new CreateSessionCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0",
            LoginMethod = method
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.LoginMethod.Should().Be(method.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateSessionCommandHandler(context);
        var command = new CreateSessionCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0",
            LoginMethod = LoginMethod.Password
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
