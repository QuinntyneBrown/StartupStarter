using FluentAssertions;
using StartupStarter.Api.Features.ProfileManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ProfileAggregate.Entities;
using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.ProfileManagement;

public class UpdateProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProfile_ShouldUpdateProfile()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var profile = new Profile(
            "profile-123",
            "Original Name",
            "acc-123",
            "admin",
            ProfileType.Personal,
            false
        );
        context.Profiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new UpdateProfileCommandHandler(context);
        var command = new UpdateProfileCommand
        {
            ProfileId = "profile-123",
            ProfileName = "Updated Name",
            AvatarUrl = null,
            UpdatedBy = "editor"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().Be("profile-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingProfile_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateProfileCommandHandler(context);
        var command = new UpdateProfileCommand
        {
            ProfileId = "non-existent-profile",
            ProfileName = "New Name",
            UpdatedBy = "editor"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateProfileCommandHandler(context);
        var command = new UpdateProfileCommand
        {
            ProfileId = "profile-123",
            ProfileName = "New Name",
            UpdatedBy = "editor"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
