using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StartupStarter.Api.Features.AccountManagement.Commands;
using StartupStarter.Api.Features.AccountManagement.Dtos;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Api.Tests.Controllers;

public class AccountsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AccountsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region CreateAccount Tests

    [Fact]
    public async Task CreateAccount_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            AccountName = "Test Company",
            AccountType = AccountType.Business,
            OwnerUserId = "owner-123",
            SubscriptionTier = "Premium",
            CreatedBy = "admin"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.AccountName.Should().Be("Test Company");
        result.AccountType.Should().Be(AccountType.Business.ToString());
        result.OwnerUserId.Should().Be("owner-123");
        result.SubscriptionTier.Should().Be("Premium");
        result.Status.Should().Be(AccountStatus.Active.ToString());
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnLocationHeader()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            AccountName = "Location Header Company",
            AccountType = AccountType.Individual,
            OwnerUserId = "owner-456",
            SubscriptionTier = "Basic",
            CreatedBy = "system"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/accounts/");
    }

    [Theory]
    [InlineData(AccountType.Individual)]
    [InlineData(AccountType.Business)]
    [InlineData(AccountType.Enterprise)]
    public async Task CreateAccount_WithDifferentTypes_ShouldSucceed(AccountType accountType)
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            AccountName = $"Account Type {accountType}",
            AccountType = accountType,
            OwnerUserId = "owner-789",
            SubscriptionTier = "Standard",
            CreatedBy = "admin"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.AccountType.Should().Be(accountType.ToString());
    }

    #endregion

    #region GetAccount Tests

    [Fact]
    public async Task GetAccount_WithExistingAccount_ShouldReturnOk()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IStartupStarterContext>();

        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Get Test Company",
            AccountType.Business,
            "owner-123",
            "Premium",
            "admin");
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/accounts/{accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.AccountId.Should().Be(accountId);
        result.AccountName.Should().Be("Get Test Company");
        result.Status.Should().Be(AccountStatus.Active.ToString());
    }

    [Fact]
    public async Task GetAccount_WithNonExistingAccount_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/accounts/non-existing-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAccount_WithSuspendedAccount_ShouldReturnSuspensionDetails()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IStartupStarterContext>();

        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Suspended Company",
            AccountType.Business,
            "owner-123",
            "Premium",
            "admin");
        account.Suspend("Policy violation", "compliance");
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/accounts/{accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.Status.Should().Be(AccountStatus.Suspended.ToString());
        result.SuspensionReason.Should().Be("Policy violation");
    }

    [Fact]
    public async Task GetAccount_WithUpdatedSubscription_ShouldReturnCurrentTier()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IStartupStarterContext>();

        var accountId = Guid.NewGuid().ToString();
        var account = new Account(
            accountId,
            "Upgraded Company",
            AccountType.Business,
            "owner-123",
            "Basic",
            "admin");
        account.ChangeSubscriptionTier("Enterprise", "sales");
        account.ClearDomainEvents();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/accounts/{accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.SubscriptionTier.Should().Be("Enterprise");
    }

    #endregion

    #region End-to-End Tests

    [Fact]
    public async Task CreateAndGetAccount_ShouldWorkEndToEnd()
    {
        // Arrange - Create
        var createCommand = new CreateAccountCommand
        {
            AccountName = "End-to-End Company",
            AccountType = AccountType.Enterprise,
            OwnerUserId = "e2e-owner",
            SubscriptionTier = "Enterprise",
            CreatedBy = "admin"
        };

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/accounts", createCommand);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdAccount = await createResponse.Content.ReadFromJsonAsync<AccountDto>();
        createdAccount.Should().NotBeNull();

        // Act - Get
        var getResponse = await _client.GetAsync($"/api/accounts/{createdAccount!.AccountId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedAccount = await getResponse.Content.ReadFromJsonAsync<AccountDto>();

        // Assert
        retrievedAccount.Should().NotBeNull();
        retrievedAccount!.AccountId.Should().Be(createdAccount.AccountId);
        retrievedAccount.AccountName.Should().Be("End-to-End Company");
        retrievedAccount.AccountType.Should().Be(AccountType.Enterprise.ToString());
        retrievedAccount.OwnerUserId.Should().Be("e2e-owner");
        retrievedAccount.SubscriptionTier.Should().Be("Enterprise");
    }

    #endregion
}
