using FluentAssertions;
using StartupStarter.Api.Features.ApiManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ApiManagement;

public class RevokeApiKeyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingApiKey_ShouldRevokeApiKey()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var apiKey = new ApiKey(
            "key-123",
            "Test Key",
            "acc-123",
            "hash-abc",
            "creator",
            new List<string> { "read" },
            DateTime.UtcNow.AddDays(30)
        );
        context.ApiKeys.Add(apiKey);
        await context.SaveChangesAsync();

        var handler = new RevokeApiKeyCommandHandler(context);
        var command = new RevokeApiKeyCommand
        {
            ApiKeyId = "key-123",
            RevokedBy = "admin",
            Reason = "Security concern"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ApiKeyId.Should().Be("key-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingApiKey_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RevokeApiKeyCommandHandler(context);
        var command = new RevokeApiKeyCommand
        {
            ApiKeyId = "non-existent-key",
            RevokedBy = "admin",
            Reason = "Test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldPersistRevocationToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var apiKey = new ApiKey(
            "key-456",
            "Revoke Test Key",
            "acc-456",
            "hash-def",
            "creator",
            new List<string> { "write" },
            null
        );
        context.ApiKeys.Add(apiKey);
        await context.SaveChangesAsync();

        var handler = new RevokeApiKeyCommandHandler(context);
        var command = new RevokeApiKeyCommand
        {
            ApiKeyId = "key-456",
            RevokedBy = "security-team",
            Reason = "Compromised key"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedApiKey = await context.ApiKeys.FindAsync("key-456");
        savedApiKey.Should().NotBeNull();
        savedApiKey!.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RevokeApiKeyCommandHandler(context);
        var command = new RevokeApiKeyCommand
        {
            ApiKeyId = "key-123",
            RevokedBy = "admin",
            Reason = "Test"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
