using StartupStarter.Core.Model.DashboardAggregate.Enums;

namespace StartupStarter.Core.Model.DashboardAggregate.Entities;

public class DashboardShare
{
    public string DashboardShareId { get; private set; }
    public string DashboardId { get; private set; }
    public string OwnerUserId { get; private set; }
    public string SharedWithUserId { get; private set; }
    public PermissionLevel PermissionLevel { get; private set; }
    public DateTime SharedAt { get; private set; }

    public Dashboard Dashboard { get; private set; } = null!;

    // EF Core constructor
    private DashboardShare()
    {
        DashboardShareId = string.Empty;
        DashboardId = string.Empty;
        OwnerUserId = string.Empty;
        SharedWithUserId = string.Empty;
    }

    public DashboardShare(string dashboardShareId, string dashboardId, string ownerUserId,
        string sharedWithUserId, PermissionLevel permissionLevel)
    {
        if (string.IsNullOrWhiteSpace(dashboardShareId))
            throw new ArgumentException("DashboardShare ID cannot be empty", nameof(dashboardShareId));
        if (string.IsNullOrWhiteSpace(dashboardId))
            throw new ArgumentException("Dashboard ID cannot be empty", nameof(dashboardId));
        if (string.IsNullOrWhiteSpace(ownerUserId))
            throw new ArgumentException("Owner user ID cannot be empty", nameof(ownerUserId));
        if (string.IsNullOrWhiteSpace(sharedWithUserId))
            throw new ArgumentException("Shared with user ID cannot be empty", nameof(sharedWithUserId));

        DashboardShareId = dashboardShareId;
        DashboardId = dashboardId;
        OwnerUserId = ownerUserId;
        SharedWithUserId = sharedWithUserId;
        PermissionLevel = permissionLevel;
        SharedAt = DateTime.UtcNow;
    }
}
