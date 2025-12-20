using FluentAssertions;
using StartupStarter.Api.Features.UserManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.UserManagement;

public class GetUserInvitationByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingInvitation_ShouldReturnInvitation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var invitation = new UserInvitation(
            "invitation-123",
            "invitee@example.com",
            "acc-123",
            "admin",
            new List<string> { "role-1" },
            DateTime.UtcNow.AddDays(7)
        );
        context.UserInvitations.Add(invitation);
        await context.SaveChangesAsync();

        var handler = new GetUserInvitationByIdQueryHandler(context);
        var query = new GetUserInvitationByIdQuery { InvitationId = "invitation-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.InvitationId.Should().Be("invitation-123");
        result.Email.Should().Be("invitee@example.com");
    }

    [Fact]
    public async Task Handle_WithNonExistingInvitation_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetUserInvitationByIdQueryHandler(context);
        var query = new GetUserInvitationByIdQuery { InvitationId = "non-existent-invitation" };

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
        var handler = new GetUserInvitationByIdQueryHandler(context);
        var query = new GetUserInvitationByIdQuery { InvitationId = "invitation-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
