using FluentAssertions;
using StartupStarter.Api.Features.MediaManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.MediaAggregate.Entities;
using StartupStarter.Core.Model.MediaAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.MediaManagement;

public class DeleteMediaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingMedia_ShouldDeleteMedia()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var media = new Media(
            "media-123",
            "to-delete.png",
            "image/png",
            1000,
            "user-123",
            "acc-123",
            "profile-123",
            "/uploads/to-delete.png"
        );
        context.Medias.Add(media);
        await context.SaveChangesAsync();

        var handler = new DeleteMediaCommandHandler(context);
        var command = new DeleteMediaCommand
        {
            MediaId = "media-123",
            DeletedBy = "admin",
            DeletionType = DeletionType.SoftDelete
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistingMedia_ShouldReturnFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new DeleteMediaCommandHandler(context);
        var command = new DeleteMediaCommand
        {
            MediaId = "non-existent-media",
            DeletedBy = "admin",
            DeletionType = DeletionType.SoftDelete
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(DeletionType.SoftDelete)]
    [InlineData(DeletionType.HardDelete)]
    public async Task Handle_WithDifferentDeletionTypes_ShouldDeleteMedia(DeletionType deletionType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var media = new Media(
            $"media-{deletionType}",
            "file.png",
            "image/png",
            1000,
            "user-123",
            "acc-123",
            "profile-123",
            "/uploads/file.png"
        );
        context.Medias.Add(media);
        await context.SaveChangesAsync();

        var handler = new DeleteMediaCommandHandler(context);
        var command = new DeleteMediaCommand
        {
            MediaId = $"media-{deletionType}",
            DeletedBy = "admin",
            DeletionType = deletionType
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new DeleteMediaCommandHandler(context);
        var command = new DeleteMediaCommand
        {
            MediaId = "media-123",
            DeletedBy = "admin",
            DeletionType = DeletionType.SoftDelete
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
