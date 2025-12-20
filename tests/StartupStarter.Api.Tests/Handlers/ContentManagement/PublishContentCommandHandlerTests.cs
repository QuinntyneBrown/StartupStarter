using FluentAssertions;
using StartupStarter.Api.Features.ContentManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ContentManagement;

public class PublishContentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingContent_ShouldPublishContent()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var content = new Content(
            "content-123",
            "Article",
            "Draft Article",
            "Draft body",
            "author-123",
            "acc-123",
            "profile-123"
        );
        context.Contents.Add(content);
        await context.SaveChangesAsync();

        var handler = new PublishContentCommandHandler(context);
        var command = new PublishContentCommand
        {
            ContentId = "content-123",
            PublishedBy = "publisher",
            PublishDate = DateTime.UtcNow
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ContentId.Should().Be("content-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingContent_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new PublishContentCommandHandler(context);
        var command = new PublishContentCommand
        {
            ContentId = "non-existent-content",
            PublishedBy = "publisher",
            PublishDate = DateTime.UtcNow
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldPersistPublishToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var content = new Content(
            "content-456",
            "BlogPost",
            "Post Title",
            "Post body",
            "author-456",
            "acc-456",
            "profile-456"
        );
        context.Contents.Add(content);
        await context.SaveChangesAsync();

        var handler = new PublishContentCommandHandler(context);
        var publishDate = DateTime.UtcNow;
        var command = new PublishContentCommand
        {
            ContentId = "content-456",
            PublishedBy = "editor",
            PublishDate = publishDate
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedContent = await context.Contents.FindAsync("content-456");
        savedContent.Should().NotBeNull();
        savedContent!.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new PublishContentCommandHandler(context);
        var command = new PublishContentCommand
        {
            ContentId = "content-123",
            PublishedBy = "publisher",
            PublishDate = DateTime.UtcNow
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
