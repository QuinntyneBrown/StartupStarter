using FluentAssertions;
using StartupStarter.Api.Features.ContentManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ContentManagement;

public class UpdateContentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingContent_ShouldUpdateContent()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var content = new Content(
            "content-123",
            "Article",
            "Original Title",
            "Original body",
            "author-123",
            "acc-123",
            "profile-123"
        );
        context.Contents.Add(content);
        await context.SaveChangesAsync();

        var handler = new UpdateContentCommandHandler(context);
        var command = new UpdateContentCommand
        {
            ContentId = "content-123",
            Title = "Updated Title",
            Body = "Updated body",
            UpdatedBy = "editor"
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
        var handler = new UpdateContentCommandHandler(context);
        var command = new UpdateContentCommand
        {
            ContentId = "non-existent-content",
            Title = "New Title",
            UpdatedBy = "editor"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldPersistUpdateToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var content = new Content(
            "content-456",
            "BlogPost",
            "Old Title",
            "Old body",
            "author-456",
            "acc-456",
            "profile-456"
        );
        context.Contents.Add(content);
        await context.SaveChangesAsync();

        var handler = new UpdateContentCommandHandler(context);
        var command = new UpdateContentCommand
        {
            ContentId = "content-456",
            Title = "New Title",
            Body = "New body content",
            UpdatedBy = "editor"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedContent = await context.Contents.FindAsync("content-456");
        savedContent.Should().NotBeNull();
        savedContent!.Title.Should().Be("New Title");
        savedContent.Body.Should().Be("New body content");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UpdateContentCommandHandler(context);
        var command = new UpdateContentCommand
        {
            ContentId = "content-123",
            Title = "New Title",
            UpdatedBy = "editor"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
