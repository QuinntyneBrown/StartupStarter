using FluentAssertions;
using StartupStarter.Api.Features.ProfileManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ProfileAggregate.Entities;
using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.ProfileManagement;

public class GetProfileByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProfile_ShouldReturnProfile()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var profile = new Profile(
            "profile-123",
            "Test Profile",
            "acc-123",
            "admin",
            ProfileType.Business,
            true
        );
        context.Profiles.Add(profile);
        await context.SaveChangesAsync();

        var handler = new GetProfileByIdQueryHandler(context);
        var query = new GetProfileByIdQuery { ProfileId = "profile-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().Be("profile-123");
        result.ProfileName.Should().Be("Test Profile");
    }

    [Fact]
    public async Task Handle_WithNonExistingProfile_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetProfileByIdQueryHandler(context);
        var query = new GetProfileByIdQuery { ProfileId = "non-existent-profile" };

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
        var handler = new GetProfileByIdQueryHandler(context);
        var query = new GetProfileByIdQuery { ProfileId = "profile-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
