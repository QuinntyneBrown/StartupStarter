using FluentAssertions;
using StartupStarter.Api.Features.ContentManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ContentManagement;

public class GetContentByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingContent_ShouldReturnContent()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var content = new Content(
            "content-123",
            "Article",
            "Test Article",
            "Article body",
            "author-123",
            "acc-123",
            "profile-123"
        );
        context.Contents.Add(content);
        await context.SaveChangesAsync();

        var handler = new GetContentByIdQueryHandler(context);
        var query = new GetContentByIdQuery { ContentId = "content-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ContentId.Should().Be("content-123");
        result.Title.Should().Be("Test Article");
        result.ContentType.Should().Be("Article");
    }

    [Fact]
    public async Task Handle_WithNonExistingContent_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetContentByIdQueryHandler(context);
        var query = new GetContentByIdQuery { ContentId = "non-existent-content" };

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
        var handler = new GetContentByIdQueryHandler(context);
        var query = new GetContentByIdQuery { ContentId = "content-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
