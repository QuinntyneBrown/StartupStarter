using FluentAssertions;
using StartupStarter.Api.Features.RoleManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.RoleManagement;

public class GetUserRolesByUserIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingUserRoles_ShouldReturnUserRoles()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userRole1 = new UserRole("ur-1", "role-1", "user-123", "acc-123", "admin");
        var userRole2 = new UserRole("ur-2", "role-2", "user-123", "acc-123", "admin");
        context.UserRoles.Add(userRole1);
        context.UserRoles.Add(userRole2);
        await context.SaveChangesAsync();

        var handler = new GetUserRolesByUserIdQueryHandler(context);
        var query = new GetUserRolesByUserIdQuery { UserId = "user-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoUserRoles_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetUserRolesByUserIdQueryHandler(context);
        var query = new GetUserRolesByUserIdQuery { UserId = "user-with-no-roles" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldOnlyReturnActiveRoles()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var activeRole = new UserRole("ur-1", "role-1", "user-456", "acc-123", "admin");
        var inactiveRole = new UserRole("ur-2", "role-2", "user-456", "acc-123", "admin");
        inactiveRole.Deactivate("admin");
        context.UserRoles.Add(activeRole);
        context.UserRoles.Add(inactiveRole);
        await context.SaveChangesAsync();

        var handler = new GetUserRolesByUserIdQueryHandler(context);
        var query = new GetUserRolesByUserIdQuery { UserId = "user-456" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].RoleId.Should().Be("role-1");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetUserRolesByUserIdQueryHandler(context);
        var query = new GetUserRolesByUserIdQuery { UserId = "user-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
