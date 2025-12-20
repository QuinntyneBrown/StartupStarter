using FluentAssertions;
using Moq;
using StartupStarter.Api.Authentication;
using StartupStarter.Api.Features.AuthManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuthManagement;

public class LoginCommandHandlerTests
{
    private Mock<IJwtTokenService> CreateMockJwtTokenService()
    {
        var mock = new Mock<IJwtTokenService>();
        mock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
            .Returns("test-access-token");
        mock.Setup(x => x.GenerateRefreshToken())
            .Returns("test-refresh-token");
        return mock;
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnLoginDto()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            "user-123",
            "test@example.com",
            "John",
            "Doe",
            "acc-123",
            "password123",
            new List<string>(),
            "system",
            true
        );
        user.Activate("system", ActivationMethod.EmailVerification);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "password123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be("user-123");
        result.Email.Should().Be("test@example.com");
        result.AccessToken.Should().Be("test-access-token");
        result.RefreshToken.Should().Be("test-refresh-token");
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "nonexistent@example.com",
            Password = "password123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            "user-123",
            "test@example.com",
            "John",
            "Doe",
            "acc-123",
            "correctpassword",
            new List<string>(),
            "system",
            true
        );
        user.Activate("system", ActivationMethod.EmailVerification);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "wrongpassword",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            "user-123",
            "test@example.com",
            "John",
            "Doe",
            "acc-123",
            "password123",
            new List<string>(),
            "system",
            true
        );
        // User is not activated
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "password123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithFailedLogin_ShouldRecordLoginAttempt()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "nonexistent@example.com",
            Password = "password",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.LoginAttempts.Should().HaveCount(1);
        var attempt = context.LoginAttempts.First();
        attempt.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithSuccessfulLogin_ShouldCreateSession()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            "user-123",
            "test@example.com",
            "John",
            "Doe",
            "acc-123",
            "password123",
            new List<string>(),
            "system",
            true
        );
        user.Activate("system", ActivationMethod.EmailVerification);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "password123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent/1.0"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.UserSessions.Should().HaveCount(1);
        var session = context.UserSessions.First();
        session.UserId.Should().Be("user-123");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var jwtTokenService = CreateMockJwtTokenService();
        var handler = new LoginCommandHandler(context, jwtTokenService.Object);
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "password123"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
