using FluentAssertions;
using StartupStarter.Api.Features.AccountManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AccountManagement;

public class GetAccountByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingAccount_ShouldReturnAccountDto()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Test Company",
            AccountType.Business,
            "owner-123",
            "Premium",
            "admin");
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccountId.Should().Be(accountId);
        result.AccountName.Should().Be("Test Company");
        result.AccountType.Should().Be(AccountType.Business.ToString());
        result.OwnerUserId.Should().Be("owner-123");
        result.SubscriptionTier.Should().Be("Premium");
        result.Status.Should().Be(AccountStatus.Active.ToString());
    }

    [Fact]
    public async Task Handle_WithNonExistingAccount_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = "non-existing-account-id" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithEmptyAccountId_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = "" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithSuspendedAccount_ShouldReturnSuspensionDetails()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Suspended Company",
            AccountType.Business,
            "owner-123",
            "Premium",
            "admin");
        account.Suspend("Policy violation", "compliance", TimeSpan.FromDays(30));
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(AccountStatus.Suspended.ToString());
        result.SuspendedAt.Should().NotBeNull();
        result.SuspensionReason.Should().Be("Policy violation");
    }

    [Fact]
    public async Task Handle_WithDeletedAccount_ShouldReturnDeletedStatus()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Deleted Company",
            AccountType.Individual,
            "owner-123",
            "Basic",
            "admin");
        account.Delete("admin", DeletionType.Soft);
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(AccountStatus.Deleted.ToString());
        result.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldMapAllAccountFieldsCorrectly()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Complete Company",
            AccountType.Enterprise,
            "owner-full",
            "Enterprise",
            "admin");
        account.UpdateAccountInfo("Updated Company Name", "manager");
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccountId.Should().Be(accountId);
        result.AccountName.Should().Be("Updated Company Name");
        result.AccountType.Should().Be(AccountType.Enterprise.ToString());
        result.OwnerUserId.Should().Be("owner-full");
        result.SubscriptionTier.Should().Be("Enterprise");
        result.Status.Should().Be(AccountStatus.Active.ToString());
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithMultipleAccounts_ShouldReturnCorrectAccount()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId1 = Guid.NewGuid().ToString();
        var accountId2 = Guid.NewGuid().ToString();
        var accountId3 = Guid.NewGuid().ToString();

        var account1 = new Account(accountId1, "Company One", AccountType.Business, "owner-1", "Basic", "admin");
        var account2 = new Account(accountId2, "Company Two", AccountType.Individual, "owner-2", "Premium", "admin");
        var account3 = new Account(accountId3, "Company Three", AccountType.Enterprise, "owner-3", "Enterprise", "admin");

        context.Accounts.AddRange(account1, account2, account3);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId2 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccountId.Should().Be(accountId2);
        result.AccountName.Should().Be("Company Two");
        result.AccountType.Should().Be(AccountType.Individual.ToString());
        result.SubscriptionTier.Should().Be("Premium");
    }

    [Fact]
    public async Task Handle_AfterOwnershipTransfer_ShouldReturnNewOwner()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Transfer Company",
            AccountType.Business,
            "original-owner",
            "Premium",
            "admin");
        account.TransferOwnership("new-owner", "admin");
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.OwnerUserId.Should().Be("new-owner");
    }

    [Fact]
    public async Task Handle_AfterSubscriptionChange_ShouldReturnNewTier()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Upgrade Company",
            AccountType.Business,
            "owner-123",
            "Basic",
            "admin");
        account.ChangeSubscriptionTier("Enterprise", "sales");
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = accountId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.SubscriptionTier.Should().Be("Enterprise");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetAccountByIdQueryHandler(context);
        var query = new GetAccountByIdQuery { AccountId = "some-id" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
