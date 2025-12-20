using FluentAssertions;
using StartupStarter.Api.Features.WebhookManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.WebhookManagement;

public class RegisterWebhookCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldRegisterWebhook()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RegisterWebhookCommandHandler(context);
        var command = new RegisterWebhookCommand
        {
            Url = "https://example.com/webhook",
            AccountId = "acc-123",
            EventTypes = new List<string> { "user.created", "user.updated" },
            RegisteredBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be("https://example.com/webhook");
        result.WebhookId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistWebhookToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RegisterWebhookCommandHandler(context);
        var command = new RegisterWebhookCommand
        {
            Url = "https://api.example.com/hooks",
            AccountId = "acc-456",
            EventTypes = new List<string> { "content.published" },
            RegisteredBy = "system"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedWebhook = await context.Webhooks.FindAsync(result.WebhookId);
        savedWebhook.Should().NotBeNull();
        savedWebhook!.Url.Should().Be("https://api.example.com/hooks");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueWebhookId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RegisterWebhookCommandHandler(context);
        var command1 = new RegisterWebhookCommand
        {
            Url = "https://example1.com/webhook",
            AccountId = "acc-123",
            EventTypes = new List<string> { "event1" },
            RegisteredBy = "admin"
        };
        var command2 = new RegisterWebhookCommand
        {
            Url = "https://example2.com/webhook",
            AccountId = "acc-123",
            EventTypes = new List<string> { "event2" },
            RegisteredBy = "admin"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.WebhookId.Should().NotBe(result2.WebhookId);
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RegisterWebhookCommandHandler(context);
        var command = new RegisterWebhookCommand
        {
            Url = "https://example.com/webhook",
            AccountId = "acc-123",
            EventTypes = new List<string>(),
            RegisteredBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
