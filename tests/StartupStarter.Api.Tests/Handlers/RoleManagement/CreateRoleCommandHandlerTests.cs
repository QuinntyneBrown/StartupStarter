using FluentAssertions;
using StartupStarter.Api.Features.RoleManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.RoleManagement;

public class CreateRoleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateRole()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateRoleCommandHandler(context);
        var command = new CreateRoleCommand
        {
            RoleName = "Admin",
            Description = "Administrator role",
            AccountId = "acc-123",
            Permissions = new List<string> { "read", "write", "delete" },
            CreatedBy = "system"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RoleName.Should().Be("Admin");
        result.RoleId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistRoleToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateRoleCommandHandler(context);
        var command = new CreateRoleCommand
        {
            RoleName = "Editor",
            Description = "Content editor role",
            AccountId = "acc-456",
            Permissions = new List<string> { "read", "write" },
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedRole = await context.Roles.FindAsync(result.RoleId);
        savedRole.Should().NotBeNull();
        savedRole!.RoleName.Should().Be("Editor");
        savedRole.Description.Should().Be("Content editor role");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueRoleId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateRoleCommandHandler(context);
        var command1 = new CreateRoleCommand
        {
            RoleName = "Role One",
            Description = "First role",
            AccountId = "acc-123",
            Permissions = new List<string>(),
            CreatedBy = "admin"
        };
        var command2 = new CreateRoleCommand
        {
            RoleName = "Role Two",
            Description = "Second role",
            AccountId = "acc-123",
            Permissions = new List<string>(),
            CreatedBy = "admin"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.RoleId.Should().NotBe(result2.RoleId);
    }

    [Fact]
    public async Task Handle_WithPermissions_ShouldStorePermissionsCorrectly()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateRoleCommandHandler(context);
        var permissions = new List<string> { "users:read", "users:write", "content:manage" };
        var command = new CreateRoleCommand
        {
            RoleName = "Manager",
            Description = "Manager role",
            AccountId = "acc-123",
            Permissions = permissions,
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedRole = await context.Roles.FindAsync(result.RoleId);
        savedRole.Should().NotBeNull();
        savedRole!.Permissions.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateRoleCommandHandler(context);
        var command = new CreateRoleCommand
        {
            RoleName = "Cancel Test",
            Description = "Test",
            AccountId = "acc-123",
            Permissions = new List<string>(),
            CreatedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
