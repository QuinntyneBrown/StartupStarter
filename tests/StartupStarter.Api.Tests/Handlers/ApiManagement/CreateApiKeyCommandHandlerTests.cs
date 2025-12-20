using FluentAssertions;
using StartupStarter.Api.Features.ApiManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.ApiManagement;

public class CreateApiKeyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateApiKey()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateApiKeyCommandHandler(context);
        var command = new CreateApiKeyCommand
        {
            KeyName = "Test API Key",
            AccountId = "acc-123",
            KeyHash = "hash-abc123",
            Permissions = new List<string> { "read", "write" },
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.KeyName.Should().Be("Test API Key");
        result.AccountId.Should().Be("acc-123");
        result.ApiKeyId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistApiKeyToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateApiKeyCommandHandler(context);
        var command = new CreateApiKeyCommand
        {
            KeyName = "Persistent Key",
            AccountId = "acc-456",
            KeyHash = "hash-def456",
            Permissions = new List<string> { "read" },
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedBy = "system"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedApiKey = await context.ApiKeys.FindAsync(result.ApiKeyId);
        savedApiKey.Should().NotBeNull();
        savedApiKey!.KeyName.Should().Be("Persistent Key");
        savedApiKey.AccountId.Should().Be("acc-456");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueApiKeyId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateApiKeyCommandHandler(context);
        var command1 = new CreateApiKeyCommand
        {
            KeyName = "Key One",
            AccountId = "acc-123",
            KeyHash = "hash-1",
            Permissions = new List<string> { "read" },
            CreatedBy = "admin"
        };
        var command2 = new CreateApiKeyCommand
        {
            KeyName = "Key Two",
            AccountId = "acc-123",
            KeyHash = "hash-2",
            Permissions = new List<string> { "write" },
            CreatedBy = "admin"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.ApiKeyId.Should().NotBe(result2.ApiKeyId);
    }

    [Fact]
    public async Task Handle_WithPermissions_ShouldStorePermissionsCorrectly()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateApiKeyCommandHandler(context);
        var permissions = new List<string> { "read", "write", "delete" };
        var command = new CreateApiKeyCommand
        {
            KeyName = "Full Access Key",
            AccountId = "acc-789",
            KeyHash = "hash-ghi789",
            Permissions = permissions,
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedApiKey = await context.ApiKeys.FindAsync(result.ApiKeyId);
        savedApiKey.Should().NotBeNull();
        savedApiKey!.Permissions.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateApiKeyCommandHandler(context);
        var command = new CreateApiKeyCommand
        {
            KeyName = "Cancel Test",
            AccountId = "acc-123",
            KeyHash = "hash-cancel",
            Permissions = new List<string>(),
            CreatedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
