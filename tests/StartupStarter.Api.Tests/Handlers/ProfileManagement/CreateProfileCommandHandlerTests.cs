using FluentAssertions;
using StartupStarter.Api.Features.ProfileManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.ProfileManagement;

public class CreateProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProfile()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateProfileCommandHandler(context);
        var command = new CreateProfileCommand
        {
            ProfileName = "Test Profile",
            AccountId = "acc-123",
            ProfileType = ProfileType.Business,
            IsDefault = true,
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProfileName.Should().Be("Test Profile");
        result.ProfileId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistProfileToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateProfileCommandHandler(context);
        var command = new CreateProfileCommand
        {
            ProfileName = "Persistent Profile",
            AccountId = "acc-456",
            ProfileType = ProfileType.Personal,
            IsDefault = false,
            CreatedBy = "system"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedProfile = await context.Profiles.FindAsync(result.ProfileId);
        savedProfile.Should().NotBeNull();
        savedProfile!.ProfileName.Should().Be("Persistent Profile");
    }

    [Theory]
    [InlineData(ProfileType.Personal)]
    [InlineData(ProfileType.Business)]
    public async Task Handle_WithDifferentProfileTypes_ShouldCreateCorrectType(ProfileType profileType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateProfileCommandHandler(context);
        var command = new CreateProfileCommand
        {
            ProfileName = $"Profile {profileType}",
            AccountId = "acc-123",
            ProfileType = profileType,
            IsDefault = false,
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProfileType.Should().Be(profileType.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateProfileCommandHandler(context);
        var command = new CreateProfileCommand
        {
            ProfileName = "Cancel Test",
            AccountId = "acc-123",
            ProfileType = ProfileType.Personal,
            IsDefault = false,
            CreatedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
