using FluentAssertions;
using StartupStarter.Api.Features.RoleManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.RoleManagement;

public class GetRoleByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingRole_ShouldReturnRole()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var role = new Role(
            "role-123",
            "Admin",
            "Administrator role",
            "acc-123",
            new List<string> { "read", "write", "delete" },
            "system"
        );
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new GetRoleByIdQueryHandler(context);
        var query = new GetRoleByIdQuery { RoleId = "role-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.RoleId.Should().Be("role-123");
        result.RoleName.Should().Be("Admin");
    }

    [Fact]
    public async Task Handle_WithNonExistingRole_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetRoleByIdQueryHandler(context);
        var query = new GetRoleByIdQuery { RoleId = "non-existent-role" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetRoleByIdQueryHandler(context);
        var query = new GetRoleByIdQuery { RoleId = "role-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
