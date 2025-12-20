using FluentAssertions;
using StartupStarter.Api.Features.ContentManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ContentManagement;

public class GetContentVersionByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingVersion_ShouldReturnVersion()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var contentVersion = new ContentVersion(
            "version-123",
            "content-123",
            1,
            "Version 1 Title",
            "Version 1 body",
            "author-123"
        );
        context.ContentVersions.Add(contentVersion);
        await context.SaveChangesAsync();

        var handler = new GetContentVersionByIdQueryHandler(context);
        var query = new GetContentVersionByIdQuery
        {
            ContentId = "content-123",
            VersionId = "version-123"
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ContentVersionId.Should().Be("version-123");
        result.ContentId.Should().Be("content-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingVersion_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetContentVersionByIdQueryHandler(context);
        var query = new GetContentVersionByIdQuery
        {
            ContentId = "content-123",
            VersionId = "non-existent-version"
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWrongContentId_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var contentVersion = new ContentVersion(
            "version-123",
            "content-123",
            1,
            "Version Title",
            "Version body",
            "author-123"
        );
        context.ContentVersions.Add(contentVersion);
        await context.SaveChangesAsync();

        var handler = new GetContentVersionByIdQueryHandler(context);
        var query = new GetContentVersionByIdQuery
        {
            ContentId = "wrong-content-id",
            VersionId = "version-123"
        };

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
        var handler = new GetContentVersionByIdQueryHandler(context);
        var query = new GetContentVersionByIdQuery
        {
            ContentId = "content-123",
            VersionId = "version-123"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
