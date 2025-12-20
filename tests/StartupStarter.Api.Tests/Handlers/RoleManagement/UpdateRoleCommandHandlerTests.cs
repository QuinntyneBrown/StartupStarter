using FluentAssertions;
using StartupStarter.Api.Features.RoleManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.RoleManagement;

public class UpdateRoleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingRole_ShouldUpdateRole()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var role = new Role(
            "role-123",
            "Original Name",
            "Original description",
            "acc-123",
            new List<string> { "read" },
            "admin"
        );
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new UpdateRoleCommandHandler(context);
        var command = new UpdateRoleCommand
        {
            RoleId = "role-123",
            RoleName = "Updated Name",
            Description = "Updated description",
            UpdatedBy = "editor"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be("role-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateRoleCommandHandler(context);
        var command = new UpdateRoleCommand
        {
            RoleId = "non-existent-role",
            RoleName = "New Name",
            UpdatedBy = "editor"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateRoleCommandHandler(context);
        var command = new UpdateRoleCommand
        {
            RoleId = "role-123",
            RoleName = "New Name",
            UpdatedBy = "editor"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
