using FluentAssertions;
using StartupStarter.Api.Features.AccountManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AccountManagement;

public class CreateAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateAccount()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "Test Company",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-123",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccountName.Should().Be("Test Company");
        result.AccountType.Should().Be(AccountType.Business.ToString());
        result.OwnerUserId.Should().Be("owner-123");
        result.SubscriptionTier.Should().Be("Premium");
        result.Status.Should().Be(AccountStatus.Active.ToString());
        result.AccountId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistAccountToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "Persistent Company",
            AccountType = AccountType.Individual,
            OwnerUserId = "owner-456",
            SubscriptionTier = "Basic",
            CreatedBy = "system"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedAccount = await context.Accounts.FindAsync(result.AccountId);
        savedAccount.Should().NotBeNull();
        savedAccount!.AccountName.Should().Be("Persistent Company");
        savedAccount.AccountType.Should().Be(AccountType.Individual);
        savedAccount.OwnerUserId.Should().Be("owner-456");
        savedAccount.SubscriptionTier.Should().Be("Basic");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueAccountId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command1 = new CreateAccountCommand
        {
            AccountName = "Company One",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-1",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };
        var command2 = new CreateAccountCommand
        {
            AccountName = "Company Two",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-2",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.AccountId.Should().NotBe(result2.AccountId);
    }

    [Theory]
    [InlineData(AccountType.Individual)]
    [InlineData(AccountType.Business)]
    [InlineData(AccountType.Enterprise)]
    public async Task Handle_WithDifferentAccountTypes_ShouldCreateCorrectType(AccountType accountType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = $"Account {accountType}",
            AccountType = accountType,
            OwnerUserId = "owner-123",
            SubscriptionTier = "Standard",
            CreatedBy = "admin"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccountType.Should().Be(accountType.ToString());
    }

    [Fact]
    public async Task Handle_WithInvalidAccountName_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-123",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_WithInvalidOwnerUserId_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "Valid Company",
            AccountType = AccountType.Business,
            OwnerUserId = "",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_WithInvalidSubscriptionTier_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "Valid Company",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-123",
            SubscriptionTier = "",
            CreatedBy = "admin"
        };

        // Act & Assert
        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "Time Test Company",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-123",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
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
        var handler = new CreateAccountCommandHandler(context);
        var command = new CreateAccountCommand
        {
            AccountName = "Cancel Test",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-123",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
