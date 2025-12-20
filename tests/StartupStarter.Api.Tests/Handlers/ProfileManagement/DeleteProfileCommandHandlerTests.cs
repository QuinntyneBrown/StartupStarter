using FluentAssertions;
using StartupStarter.Api.Features.ProfileManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ProfileAggregate.Entities;
using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.ProfileManagement;

public class DeleteProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProfile_ShouldDeleteProfile()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var profile = new Profile(
            "profile-123",
            "To Delete",
            "acc-123",
            "admin",
            ProfileType.Personal,
            false
        );
        context.Profiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new DeleteProfileCommandHandler(context);
        var command = new DeleteProfileCommand
        {
            ProfileId = "profile-123",
            DeletedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistingProfile_ShouldReturnFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new DeleteProfileCommandHandler(context);
        var command = new DeleteProfileCommand
        {
            ProfileId = "non-existent-profile",
            DeletedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new DeleteProfileCommandHandler(context);
        var command = new DeleteProfileCommand
        {
            ProfileId = "profile-123",
            DeletedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
