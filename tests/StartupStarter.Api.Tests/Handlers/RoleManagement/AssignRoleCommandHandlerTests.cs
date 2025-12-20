using FluentAssertions;
using StartupStarter.Api.Features.RoleManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.RoleManagement;

public class AssignRoleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingRole_ShouldAssignRole()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var role = new Role(
            "role-123",
            "Admin",
            "Admin role",
            "acc-123",
            new List<string> { "read", "write" },
            "system"
        );
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new AssignRoleCommandHandler(context);
        var command = new AssignRoleCommand
        {
            RoleId = "role-123",
            UserId = "user-456",
            AccountId = "acc-123",
            AssignedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be("role-123");
        result.UserId.Should().Be("user-456");
        result.UserRoleId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithNonExistingRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new AssignRoleCommandHandler(context);
        var command = new AssignRoleCommand
        {
            RoleId = "non-existent-role",
            UserId = "user-123",
            AccountId = "acc-123",
            AssignedBy = "admin"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldPersistUserRoleToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var role = new Role(
            "role-456",
            "Editor",
            "Editor role",
            "acc-456",
            new List<string> { "read" },
            "system"
        );
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new AssignRoleCommandHandler(context);
        var command = new AssignRoleCommand
        {
            RoleId = "role-456",
            UserId = "user-789",
            AccountId = "acc-456",
            AssignedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedUserRole = await context.UserRoles.FindAsync(result.UserRoleId);
        savedUserRole.Should().NotBeNull();
        savedUserRole!.RoleId.Should().Be("role-456");
        savedUserRole.UserId.Should().Be("user-789");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new AssignRoleCommandHandler(context);
        var command = new AssignRoleCommand
        {
            RoleId = "role-123",
            UserId = "user-123",
            AccountId = "acc-123",
            AssignedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
