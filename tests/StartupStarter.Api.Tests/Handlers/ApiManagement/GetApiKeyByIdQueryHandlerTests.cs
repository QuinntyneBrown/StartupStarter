using FluentAssertions;
using StartupStarter.Api.Features.ApiManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ApiManagement;

public class GetApiKeyByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingApiKey_ShouldReturnApiKey()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var apiKey = new ApiKey(
            "key-123",
            "Test Key",
            "acc-123",
            "hash-abc",
            "creator",
            new List<string> { "read", "write" },
            DateTime.UtcNow.AddDays(30)
        );
        context.ApiKeys.Add(apiKey);
        await context.SaveChangesAsync();

        var handler = new GetApiKeyByIdQueryHandler(context);
        var query = new GetApiKeyByIdQuery { ApiKeyId = "key-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ApiKeyId.Should().Be("key-123");
        result.KeyName.Should().Be("Test Key");
        result.AccountId.Should().Be("acc-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingApiKey_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetApiKeyByIdQueryHandler(context);
        var query = new GetApiKeyByIdQuery { ApiKeyId = "non-existent-key" };

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
        var handler = new GetApiKeyByIdQueryHandler(context);
        var query = new GetApiKeyByIdQuery { ApiKeyId = "key-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
