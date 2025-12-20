using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.ProfileAggregate.Entities;
using StartupStarter.Core.Model.RoleAggregate.Entities;
using StartupStarter.Core.Model.ContentAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.MediaAggregate.Entities;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;
using StartupStarter.Core.Model.WebhookAggregate.Entities;
using StartupStarter.Core.Model.AuditAggregate.Entities;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;
using StartupStarter.Core.Model.BackupAggregate.Entities;
using StartupStarter.Core.Model.SystemErrorAggregate.Entities;

namespace StartupStarter.Infrastructure;

public class StartupStarterContext : DbContext, IStartupStarterContext
{
    public StartupStarterContext(DbContextOptions<StartupStarterContext> options)
        : base(options)
    {
    }

    // Account Management
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<AccountSettings> AccountSettings { get; set; } = null!;

    // User Management
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserInvitation> UserInvitations { get; set; } = null!;

    // Profile Management
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<ProfilePreferences> ProfilePreferences { get; set; } = null!;
    public DbSet<ProfileShare> ProfileShares { get; set; } = null!;

    // Role Management
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;

    // Content Management
    public DbSet<Content> Contents { get; set; } = null!;
    public DbSet<ContentVersion> ContentVersions { get; set; } = null!;

    // Dashboard Management
    public DbSet<Dashboard> Dashboards { get; set; } = null!;
    public DbSet<DashboardCard> DashboardCards { get; set; } = null!;
    public DbSet<DashboardShare> DashboardShares { get; set; } = null!;

    // Media Management
    public DbSet<Media> Medias { get; set; } = null!;

    // API Management
    public DbSet<ApiKey> ApiKeys { get; set; } = null!;
    public DbSet<ApiRequest> ApiRequests { get; set; } = null!;
    public DbSet<Webhook> Webhooks { get; set; } = null!;
    public DbSet<WebhookDelivery> WebhookDeliveries { get; set; } = null!;

    // Audit
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<AuditExport> AuditExports { get; set; } = null!;
    public DbSet<RetentionPolicy> RetentionPolicies { get; set; } = null!;

    // Authentication
    public DbSet<UserSession> UserSessions { get; set; } = null!;
    public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;
    public DbSet<MultiFactorAuthentication> MultiFactorAuthentications { get; set; } = null!;
    public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; } = null!;

    // Workflow
    public DbSet<Workflow> Workflows { get; set; } = null!;
    public DbSet<WorkflowStage> WorkflowStages { get; set; } = null!;
    public DbSet<WorkflowApproval> WorkflowApprovals { get; set; } = null!;

    // System - Maintenance
    public DbSet<SystemMaintenance> SystemMaintenances { get; set; } = null!;

    // System - Backup
    public DbSet<SystemBackup> SystemBackups { get; set; } = null!;

    // System - Error
    public DbSet<SystemError> SystemErrors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StartupStarterContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Publish domain events before saving
        // This can be implemented later with a domain event dispatcher
        return await base.SaveChangesAsync(cancellationToken);
    }
}
