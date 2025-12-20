using FluentAssertions;
using StartupStarter.Api.Features.MediaManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.MediaAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.MediaManagement;

public class GetMediaByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingMedia_ShouldReturnMedia()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var media = new Media(
            "media-123",
            "test-file.png",
            "image/png",
            2048,
            "user-123",
            "acc-123",
            "profile-123",
            "/uploads/test-file.png"
        );
        context.Medias.Add(media);
        await context.SaveChangesAsync();

        var handler = new GetMediaByIdQueryHandler(context);
        var query = new GetMediaByIdQuery { MediaId = "media-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.MediaId.Should().Be("media-123");
        result.FileName.Should().Be("test-file.png");
        result.FileType.Should().Be("image/png");
    }

    [Fact]
    public async Task Handle_WithNonExistingMedia_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetMediaByIdQueryHandler(context);
        var query = new GetMediaByIdQuery { MediaId = "non-existent-media" };

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
        var handler = new GetMediaByIdQueryHandler(context);
        var query = new GetMediaByIdQuery { MediaId = "media-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
