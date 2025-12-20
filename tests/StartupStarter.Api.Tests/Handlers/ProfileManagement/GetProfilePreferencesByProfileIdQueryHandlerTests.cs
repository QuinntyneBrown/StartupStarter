using FluentAssertions;
using StartupStarter.Api.Features.ProfileManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ProfileManagement;

public class GetProfilePreferencesByProfileIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingPreferences_ShouldReturnPreferences()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var prefs1 = new ProfilePreferences("pref-1", "profile-123", "theme", "dark");
        var prefs2 = new ProfilePreferences("pref-2", "profile-123", "language", "en");
        context.ProfilePreferences.Add(prefs1);
        context.ProfilePreferences.Add(prefs2);
        await context.SaveChangesAsync();

        var handler = new GetProfilePreferencesByProfileIdQueryHandler(context);
        var query = new GetProfilePreferencesByProfileIdQuery { ProfileId = "profile-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoPreferences_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetProfilePreferencesByProfileIdQueryHandler(context);
        var query = new GetProfilePreferencesByProfileIdQuery { ProfileId = "profile-with-no-prefs" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetProfilePreferencesByProfileIdQueryHandler(context);
        var query = new GetProfilePreferencesByProfileIdQuery { ProfileId = "profile-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
