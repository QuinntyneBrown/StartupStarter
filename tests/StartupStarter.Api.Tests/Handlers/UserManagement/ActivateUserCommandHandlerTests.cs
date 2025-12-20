using FluentAssertions;
using StartupStarter.Api.Features.UserManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.UserManagement;

public class ActivateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingUser_ShouldActivateUser()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            "user-123",
            "test@example.com",
            "John",
            "Doe",
            "acc-123",
            "hashedPassword",
            new List<string>(),
            "admin",
            true
        );
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ActivateUserCommandHandler(context);
        var command = new ActivateUserCommand
        {
            UserId = "user-123",
            ActivatedBy = "admin",
            Method = ActivationMethod.EmailVerification
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-123");
        result.Status.Should().Be(UserStatus.Active.ToString());
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ActivateUserCommandHandler(context);
        var command = new ActivateUserCommand
        {
            UserId = "non-existent-user",
            ActivatedBy = "admin",
            Method = ActivationMethod.EmailVerification
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldPersistActivationToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            "user-456",
            "activate@example.com",
            "Jane",
            "Smith",
            "acc-456",
            "hashedPassword",
            new List<string>(),
            "system",
            true
        );
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ActivateUserCommandHandler(context);
        var command = new ActivateUserCommand
        {
            UserId = "user-456",
            ActivatedBy = "admin",
            Method = ActivationMethod.AdminApproval
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedUser = await context.Users.FindAsync("user-456");
        savedUser.Should().NotBeNull();
        savedUser!.Status.Should().Be(UserStatus.Active);
    }

    [Theory]
    [InlineData(ActivationMethod.EmailVerification)]
    [InlineData(ActivationMethod.AdminApproval)]
    [InlineData(ActivationMethod.PhoneVerification)]
    public async Task Handle_WithDifferentActivationMethods_ShouldActivateUser(ActivationMethod method)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var user = new User(
            $"user-{method}",
            $"{method}@example.com",
            "Test",
            "User",
            "acc-123",
            "hashedPassword",
            new List<string>(),
            "system",
            true
        );
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ActivateUserCommandHandler(context);
        var command = new ActivateUserCommand
        {
            UserId = $"user-{method}",
            ActivatedBy = "admin",
            Method = method
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(UserStatus.Active.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new ActivateUserCommandHandler(context);
        var command = new ActivateUserCommand
        {
            UserId = "user-123",
            ActivatedBy = "admin",
            Method = ActivationMethod.EmailVerification
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
