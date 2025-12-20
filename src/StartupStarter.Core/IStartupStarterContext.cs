using Microsoft.EntityFrameworkCore;
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

namespace StartupStarter.Core;

public interface IStartupStarterContext
{
    // Account Management
    DbSet<Account> Accounts { get; set; }
    DbSet<AccountSettings> AccountSettings { get; set; }

    // User Management
    DbSet<User> Users { get; set; }
    DbSet<UserInvitation> UserInvitations { get; set; }

    // Profile Management
    DbSet<Profile> Profiles { get; set; }
    DbSet<ProfilePreferences> ProfilePreferences { get; set; }
    DbSet<ProfileShare> ProfileShares { get; set; }

    // Role Management
    DbSet<Role> Roles { get; set; }
    DbSet<UserRole> UserRoles { get; set; }

    // Content Management
    DbSet<Content> Contents { get; set; }
    DbSet<ContentVersion> ContentVersions { get; set; }

    // Dashboard Management
    DbSet<Dashboard> Dashboards { get; set; }
    DbSet<DashboardCard> DashboardCards { get; set; }
    DbSet<DashboardShare> DashboardShares { get; set; }

    // Media Management
    DbSet<Media> Medias { get; set; }

    // API Management
    DbSet<ApiKey> ApiKeys { get; set; }
    DbSet<ApiRequest> ApiRequests { get; set; }
    DbSet<Webhook> Webhooks { get; set; }
    DbSet<WebhookDelivery> WebhookDeliveries { get; set; }

    // Audit
    DbSet<AuditLog> AuditLogs { get; set; }
    DbSet<AuditExport> AuditExports { get; set; }
    DbSet<RetentionPolicy> RetentionPolicies { get; set; }

    // Authentication
    DbSet<UserSession> UserSessions { get; set; }
    DbSet<LoginAttempt> LoginAttempts { get; set; }
    DbSet<MultiFactorAuthentication> MultiFactorAuthentications { get; set; }
    DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }

    // Workflow
    DbSet<Workflow> Workflows { get; set; }
    DbSet<WorkflowStage> WorkflowStages { get; set; }
    DbSet<WorkflowApproval> WorkflowApprovals { get; set; }

    // System - Maintenance
    DbSet<SystemMaintenance> SystemMaintenances { get; set; }

    // System - Backup
    DbSet<SystemBackup> SystemBackups { get; set; }

    // System - Error
    DbSet<SystemError> SystemErrors { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
