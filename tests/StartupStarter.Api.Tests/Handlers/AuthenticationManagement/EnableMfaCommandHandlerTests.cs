using FluentAssertions;
using StartupStarter.Api.Features.AuthenticationManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuthenticationManagement;

public class EnableMfaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldEnableMfa()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new EnableMfaCommandHandler(context);
        var command = new EnableMfaCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            Method = MfaMethod.Totp,
            EnabledBy = "admin",
            SecretKey = "secret-key-123",
            BackupCodesJson = "[\"code1\",\"code2\"]"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-123");
        result.MfaId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistMfaToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new EnableMfaCommandHandler(context);
        var command = new EnableMfaCommand
        {
            UserId = "user-456",
            AccountId = "acc-456",
            Method = MfaMethod.Sms,
            EnabledBy = "system",
            SecretKey = null,
            BackupCodesJson = null
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedMfa = await context.MultiFactorAuthentications.FindAsync(result.MfaId);
        savedMfa.Should().NotBeNull();
        savedMfa!.UserId.Should().Be("user-456");
    }

    [Theory]
    [InlineData(MfaMethod.Totp)]
    [InlineData(MfaMethod.Sms)]
    [InlineData(MfaMethod.Email)]
    public async Task Handle_WithDifferentMfaMethods_ShouldEnableCorrectMethod(MfaMethod method)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new EnableMfaCommandHandler(context);
        var command = new EnableMfaCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            Method = method,
            EnabledBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Method.Should().Be(method.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new EnableMfaCommandHandler(context);
        var command = new EnableMfaCommand
        {
            UserId = "user-123",
            AccountId = "acc-123",
            Method = MfaMethod.Totp,
            EnabledBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
