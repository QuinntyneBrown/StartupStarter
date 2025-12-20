using FluentAssertions;
using StartupStarter.Api.Features.ContentManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.ContentManagement;

public class CreateContentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateContent()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateContentCommandHandler(context);
        var command = new CreateContentCommand
        {
            ContentType = "Article",
            Title = "Test Article",
            Body = "This is the body of the test article.",
            AuthorId = "author-123",
            AccountId = "acc-123",
            ProfileId = "profile-123"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ContentType.Should().Be("Article");
        result.Title.Should().Be("Test Article");
        result.ContentId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistContentToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateContentCommandHandler(context);
        var command = new CreateContentCommand
        {
            ContentType = "BlogPost",
            Title = "Persistent Post",
            Body = "This post will be saved.",
            AuthorId = "author-456",
            AccountId = "acc-456",
            ProfileId = "profile-456"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedContent = await context.Contents.FindAsync(result.ContentId);
        savedContent.Should().NotBeNull();
        savedContent!.Title.Should().Be("Persistent Post");
        savedContent.Body.Should().Be("This post will be saved.");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueContentId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateContentCommandHandler(context);
        var command1 = new CreateContentCommand
        {
            ContentType = "Article",
            Title = "Article One",
            Body = "First article",
            AuthorId = "author-123",
            AccountId = "acc-123",
            ProfileId = "profile-123"
        };
        var command2 = new CreateContentCommand
        {
            ContentType = "Article",
            Title = "Article Two",
            Body = "Second article",
            AuthorId = "author-123",
            AccountId = "acc-123",
            ProfileId = "profile-123"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.ContentId.Should().NotBe(result2.ContentId);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateContentCommandHandler(context);
        var command = new CreateContentCommand
        {
            ContentType = "Page",
            Title = "Time Test",
            Body = "Testing timestamps",
            AuthorId = "author-123",
            AccountId = "acc-123",
            ProfileId = "profile-123"
        };
        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateContentCommandHandler(context);
        var command = new CreateContentCommand
        {
            ContentType = "Article",
            Title = "Cancel Test",
            Body = "This will be cancelled",
            AuthorId = "author-123",
            AccountId = "acc-123",
            ProfileId = "profile-123"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
