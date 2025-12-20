using FluentAssertions;
using StartupStarter.Api.Features.MediaManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.MediaAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.MediaManagement;

public class UpdateMediaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingMedia_ShouldUpdateMedia()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var media = new Media(
            "media-123",
            "original.png",
            "image/png",
            1000,
            "user-123",
            "acc-123",
            "profile-123",
            "/uploads/original.png"
        );
        context.Medias.Add(media);
        await context.SaveChangesAsync();

        var handler = new UpdateMediaCommandHandler(context);
        var command = new UpdateMediaCommand
        {
            MediaId = "media-123",
            UpdatedFields = new Dictionary<string, object> { { "FileName", "updated.png" } },
            UpdatedBy = "editor"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.MediaId.Should().Be("media-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingMedia_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateMediaCommandHandler(context);
        var command = new UpdateMediaCommand
        {
            MediaId = "non-existent-media",
            UpdatedFields = new Dictionary<string, object>(),
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
        var handler = new UpdateMediaCommandHandler(context);
        var command = new UpdateMediaCommand
        {
            MediaId = "media-123",
            UpdatedFields = new Dictionary<string, object>(),
            UpdatedBy = "editor"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
