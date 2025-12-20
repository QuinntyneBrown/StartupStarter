namespace StartupStarter.Api.Authentication.Authorization;

public static class Permissions
{
    // Account Management
    public const string AccountsRead = "accounts:read";
    public const string AccountsWrite = "accounts:write";
    public const string AccountsDelete = "accounts:delete";

    // User Management
    public const string UsersRead = "users:read";
    public const string UsersWrite = "users:write";
    public const string UsersDelete = "users:delete";

    // Profile Management
    public const string ProfilesRead = "profiles:read";
    public const string ProfilesWrite = "profiles:write";
    public const string ProfilesDelete = "profiles:delete";

    // Role Management
    public const string RolesRead = "roles:read";
    public const string RolesWrite = "roles:write";
    public const string RolesDelete = "roles:delete";

    // Content Management
    public const string ContentRead = "content:read";
    public const string ContentWrite = "content:write";
    public const string ContentPublish = "content:publish";
    public const string ContentDelete = "content:delete";

    // Dashboard Management
    public const string DashboardsRead = "dashboards:read";
    public const string DashboardsWrite = "dashboards:write";
    public const string DashboardsDelete = "dashboards:delete";

    // Media Management
    public const string MediaRead = "media:read";
    public const string MediaWrite = "media:write";
    public const string MediaDelete = "media:delete";

    // API Management
    public const string ApiKeysRead = "apikeys:read";
    public const string ApiKeysWrite = "apikeys:write";
    public const string ApiKeysDelete = "apikeys:delete";

    // Webhook Management
    public const string WebhooksRead = "webhooks:read";
    public const string WebhooksWrite = "webhooks:write";
    public const string WebhooksDelete = "webhooks:delete";

    // Audit Management
    public const string AuditRead = "audit:read";
    public const string AuditExport = "audit:export";

    // Workflow Management
    public const string WorkflowsRead = "workflows:read";
    public const string WorkflowsWrite = "workflows:write";
    public const string WorkflowsApprove = "workflows:approve";

    // System Management
    public const string SystemRead = "system:read";
    public const string SystemWrite = "system:write";
    public const string SystemMaintenance = "system:maintenance";
}
