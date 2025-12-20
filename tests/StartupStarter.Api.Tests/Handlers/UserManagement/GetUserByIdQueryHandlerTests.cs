using FluentAssertions;
using StartupStarter.Api.Features.UserManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.UserManagement;

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingUser_ShouldReturnUserDto()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userId = Guid.NewGuid().ToString();
        var user = new User(
            userId,
            "test@example.com",
            "John",
            "Doe",
            "acc-123",
            "hashedPassword",
            new List<string>(),
            "admin",
            true);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.AccountId.Should().Be("acc-123");
        result.Status.Should().Be(UserStatus.Invited.ToString());
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = "non-existing-user-id" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithEmptyUserId_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = "" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithActiveUser_ShouldReturnCorrectStatus()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userId = Guid.NewGuid().ToString();
        var user = new User(
            userId,
            "active@example.com",
            "Active",
            "User",
            "acc-123",
            "hash",
            new List<string>(),
            "admin",
            true);
        user.Activate("admin", ActivationMethod.AdminActivation);
        user.ClearDomainEvents();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(UserStatus.Active.ToString());
        result.ActivatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithLockedUser_ShouldReturnLockInformation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userId = Guid.NewGuid().ToString();
        var user = new User(
            userId,
            "locked@example.com",
            "Locked",
            "User",
            "acc-123",
            "hash",
            new List<string>(),
            "admin",
            true);
        user.Activate("admin", ActivationMethod.AdminActivation);
        user.Lock("security", "Suspicious activity detected");
        user.ClearDomainEvents();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(UserStatus.Locked.ToString());
        result.LockedAt.Should().NotBeNull();
        result.LockReason.Should().Be("Suspicious activity detected");
    }

    [Fact]
    public async Task Handle_ShouldMapAllUserFieldsCorrectly()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userId = Guid.NewGuid().ToString();
        var user = new User(
            userId,
            "complete@example.com",
            "Complete",
            "User",
            "acc-full",
            "hash",
            new List<string> { "role-1", "role-2" },
            "admin",
            true);
        user.Activate("admin", ActivationMethod.AdminActivation);
        user.Deactivate("admin", "Account review");
        user.ClearDomainEvents();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Email.Should().Be("complete@example.com");
        result.FirstName.Should().Be("Complete");
        result.LastName.Should().Be("User");
        result.AccountId.Should().Be("acc-full");
        result.Status.Should().Be(UserStatus.Inactive.ToString());
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().NotBeNull();
        result.ActivatedAt.Should().NotBeNull();
        result.DeactivatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithMultipleUsers_ShouldReturnCorrectUser()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var userId3 = Guid.NewGuid().ToString();

        var user1 = new User(userId1, "user1@example.com", "User", "One", "acc-1", "hash1", new List<string>(), "admin", true);
        var user2 = new User(userId2, "user2@example.com", "User", "Two", "acc-2", "hash2", new List<string>(), "admin", true);
        var user3 = new User(userId3, "user3@example.com", "User", "Three", "acc-3", "hash3", new List<string>(), "admin", true);

        context.Users.AddRange(user1, user2, user3);
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = userId2 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId2);
        result.Email.Should().Be("user2@example.com");
        result.FirstName.Should().Be("User");
        result.LastName.Should().Be("Two");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery { UserId = "some-id" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
