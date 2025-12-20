using FluentAssertions;
using StartupStarter.Api.Features.UserManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.UserManagement;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUser()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            AccountId = "acc-123",
            PasswordHash = "hashedPassword",
            InitialRoles = new List<string> { "role-1" },
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.AccountId.Should().Be("acc-123");
        result.Status.Should().Be(UserStatus.Invited.ToString());
        result.UserId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistUserToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var command = new CreateUserCommand
        {
            Email = "persist@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            AccountId = "acc-456",
            PasswordHash = "hashedPassword",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = false
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedUser = await context.Users.FindAsync(result.UserId);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("persist@example.com");
        savedUser.FirstName.Should().Be("Jane");
        savedUser.LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueUserId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var command1 = new CreateUserCommand
        {
            Email = "user1@example.com",
            FirstName = "User",
            LastName = "One",
            AccountId = "acc-123",
            PasswordHash = "hash1",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = true
        };
        var command2 = new CreateUserCommand
        {
            Email = "user2@example.com",
            FirstName = "User",
            LastName = "Two",
            AccountId = "acc-123",
            PasswordHash = "hash2",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.UserId.Should().NotBe(result2.UserId);
    }

    [Fact]
    public async Task Handle_WithInitialRoles_ShouldAssignRolesToUser()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var roles = new List<string> { "admin", "editor", "viewer" };
        var command = new CreateUserCommand
        {
            Email = "roles@example.com",
            FirstName = "Role",
            LastName = "User",
            AccountId = "acc-789",
            PasswordHash = "hashedPassword",
            InitialRoles = roles,
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedUser = await context.Users.FindAsync(result.UserId);
        savedUser.Should().NotBeNull();
        savedUser!.RoleIds.Should().BeEquivalentTo(roles);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var command = new CreateUserCommand
        {
            Email = "",
            FirstName = "John",
            LastName = "Doe",
            AccountId = "acc-123",
            PasswordHash = "hash",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_WithInvalidAccountId_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            AccountId = "",
            PasswordHash = "hash",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateUserCommandHandler(context);
        var command = new CreateUserCommand
        {
            Email = "cancel@example.com",
            FirstName = "Cancel",
            LastName = "User",
            AccountId = "acc-123",
            PasswordHash = "hash",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = true
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
