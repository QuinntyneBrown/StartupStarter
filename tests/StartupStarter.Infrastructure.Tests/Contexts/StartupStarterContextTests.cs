using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;
using StartupStarter.Core.Model.AuditAggregate.Entities;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Core.Model.BackupAggregate.Entities;
using StartupStarter.Core.Model.ContentAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;
using StartupStarter.Core.Model.MediaAggregate.Entities;
using StartupStarter.Core.Model.ProfileAggregate.Entities;
using StartupStarter.Core.Model.RoleAggregate.Entities;
using StartupStarter.Core.Model.SystemErrorAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.WebhookAggregate.Entities;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.Contexts;

public class StartupStarterContextTests
{
    private static StartupStarterContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StartupStarterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new StartupStarterContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    #region DbSet Properties Tests

    [Fact]
    public void Accounts_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Accounts.Should().NotBeNull();
    }

    [Fact]
    public void AccountSettings_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.AccountSettings.Should().NotBeNull();
    }

    [Fact]
    public void Users_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Users.Should().NotBeNull();
    }

    [Fact]
    public void UserInvitations_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.UserInvitations.Should().NotBeNull();
    }

    [Fact]
    public void Profiles_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Profiles.Should().NotBeNull();
    }

    [Fact]
    public void ProfilePreferences_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.ProfilePreferences.Should().NotBeNull();
    }

    [Fact]
    public void ProfileShares_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.ProfileShares.Should().NotBeNull();
    }

    [Fact]
    public void Roles_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Roles.Should().NotBeNull();
    }

    [Fact]
    public void UserRoles_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.UserRoles.Should().NotBeNull();
    }

    [Fact]
    public void Contents_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Contents.Should().NotBeNull();
    }

    [Fact]
    public void ContentVersions_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.ContentVersions.Should().NotBeNull();
    }

    [Fact]
    public void Dashboards_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Dashboards.Should().NotBeNull();
    }

    [Fact]
    public void DashboardCards_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.DashboardCards.Should().NotBeNull();
    }

    [Fact]
    public void DashboardShares_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.DashboardShares.Should().NotBeNull();
    }

    [Fact]
    public void Medias_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Medias.Should().NotBeNull();
    }

    [Fact]
    public void ApiKeys_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.ApiKeys.Should().NotBeNull();
    }

    [Fact]
    public void ApiRequests_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.ApiRequests.Should().NotBeNull();
    }

    [Fact]
    public void Webhooks_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Webhooks.Should().NotBeNull();
    }

    [Fact]
    public void WebhookDeliveries_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.WebhookDeliveries.Should().NotBeNull();
    }

    [Fact]
    public void AuditLogs_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.AuditLogs.Should().NotBeNull();
    }

    [Fact]
    public void AuditExports_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.AuditExports.Should().NotBeNull();
    }

    [Fact]
    public void RetentionPolicies_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.RetentionPolicies.Should().NotBeNull();
    }

    [Fact]
    public void UserSessions_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.UserSessions.Should().NotBeNull();
    }

    [Fact]
    public void LoginAttempts_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.LoginAttempts.Should().NotBeNull();
    }

    [Fact]
    public void MultiFactorAuthentications_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.MultiFactorAuthentications.Should().NotBeNull();
    }

    [Fact]
    public void PasswordResetRequests_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.PasswordResetRequests.Should().NotBeNull();
    }

    [Fact]
    public void Workflows_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Workflows.Should().NotBeNull();
    }

    [Fact]
    public void WorkflowStages_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.WorkflowStages.Should().NotBeNull();
    }

    [Fact]
    public void WorkflowApprovals_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.WorkflowApprovals.Should().NotBeNull();
    }

    [Fact]
    public void SystemMaintenances_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.SystemMaintenances.Should().NotBeNull();
    }

    [Fact]
    public void SystemBackups_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.SystemBackups.Should().NotBeNull();
    }

    [Fact]
    public void SystemErrors_DbSet_ShouldBeAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.SystemErrors.Should().NotBeNull();
    }

    #endregion

    #region Context Interface Tests

    [Fact]
    public void Context_ShouldImplementIStartupStarterContext()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        context.Should().BeAssignableTo<Core.IStartupStarterContext>();
    }

    #endregion

    #region SaveChangesAsync Tests

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        using var context = CreateContext();
        var account = Account.Create(
            "Test Account",
            AccountType.Personal,
            "owner-123",
            "Free");

        // Act
        context.Accounts.Add(account);
        var result = await context.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
        context.Accounts.Should().ContainSingle();
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var context = CreateContext();
        var cancellationToken = new CancellationToken();
        var account = Account.Create(
            "Test Account",
            AccountType.Personal,
            "owner-123",
            "Free");

        // Act
        context.Accounts.Add(account);
        var result = await context.SaveChangesAsync(cancellationToken);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleEntities_ShouldPersistAll()
    {
        // Arrange
        using var context = CreateContext();
        var account1 = Account.Create("Account 1", AccountType.Personal, "owner-1", "Free");
        var account2 = Account.Create("Account 2", AccountType.Business, "owner-2", "Pro");

        // Act
        context.Accounts.AddRange(account1, account2);
        var result = await context.SaveChangesAsync();

        // Assert
        result.Should().Be(2);
        context.Accounts.Should().HaveCount(2);
    }

    #endregion

    #region Entity Configuration Tests

    [Fact]
    public void OnModelCreating_ShouldApplyEntityConfigurations()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var model = context.Model;

        // Assert
        model.FindEntityType(typeof(Account)).Should().NotBeNull();
        model.FindEntityType(typeof(User)).Should().NotBeNull();
        model.FindEntityType(typeof(Role)).Should().NotBeNull();
    }

    [Fact]
    public void Account_ShouldHaveCorrectKeyConfiguration()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Account));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "AccountId");
    }

    [Fact]
    public void User_ShouldHaveCorrectKeyConfiguration()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "UserId");
    }

    [Fact]
    public void Role_ShouldHaveCorrectKeyConfiguration()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Role));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(p => p.Name == "RoleId");
    }

    #endregion

    #region CRUD Operations Tests

    [Fact]
    public async Task CanAddAndRetrieveAccount()
    {
        // Arrange
        using var context = CreateContext();
        var account = Account.Create(
            "Test Account",
            AccountType.Personal,
            "owner-123",
            "Free");

        // Act
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        var retrieved = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.AccountName.Should().Be("Test Account");
        retrieved.AccountType.Should().Be(AccountType.Personal);
    }

    [Fact]
    public async Task CanAddAndRetrieveRole()
    {
        // Arrange
        using var context = CreateContext();
        var role = Role.Create("Admin", "Administrator role", "account-123");

        // Act
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        var retrieved = await context.Roles.FirstOrDefaultAsync(r => r.RoleId == role.RoleId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.RoleName.Should().Be("Admin");
    }

    [Fact]
    public async Task CanUpdateEntity()
    {
        // Arrange
        using var context = CreateContext();
        var account = Account.Create("Original Name", AccountType.Personal, "owner-123", "Free");
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Act
        account.UpdateName("Updated Name");
        await context.SaveChangesAsync();

        // Assert
        var retrieved = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == account.AccountId);
        retrieved!.AccountName.Should().Be("Updated Name");
    }

    [Fact]
    public async Task CanDeleteEntity()
    {
        // Arrange
        using var context = CreateContext();
        var account = Account.Create("To Delete", AccountType.Personal, "owner-123", "Free");
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        var accountId = account.AccountId;

        // Act
        context.Accounts.Remove(account);
        await context.SaveChangesAsync();

        // Assert
        var retrieved = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
        retrieved.Should().BeNull();
    }

    #endregion
}
