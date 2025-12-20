namespace StartupStarter.Core.Model.RoleAggregate.Entities;

public class UserRole
{
    public string UserRoleId { get; private set; }
    public string RoleId { get; private set; }
    public string UserId { get; private set; }
    public string AccountId { get; private set; }
    public string AssignedBy { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevocationReason { get; private set; }
    public bool IsActive { get; private set; }

    public Role Role { get; private set; } = null!;

    // EF Core constructor
    private UserRole()
    {
        UserRoleId = string.Empty;
        RoleId = string.Empty;
        UserId = string.Empty;
        AccountId = string.Empty;
        AssignedBy = string.Empty;
    }

    public UserRole(string userRoleId, string roleId, string userId, string accountId, string assignedBy)
    {
        if (string.IsNullOrWhiteSpace(userRoleId))
            throw new ArgumentException("UserRole ID cannot be empty", nameof(userRoleId));
        if (string.IsNullOrWhiteSpace(roleId))
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(assignedBy))
            throw new ArgumentException("AssignedBy cannot be empty", nameof(assignedBy));

        UserRoleId = userRoleId;
        RoleId = roleId;
        UserId = userId;
        AccountId = accountId;
        AssignedBy = assignedBy;
        AssignedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Revoke(string revokedBy, string reason)
    {
        if (string.IsNullOrWhiteSpace(revokedBy))
            throw new ArgumentException("RevokedBy cannot be empty", nameof(revokedBy));

        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy;
        RevocationReason = reason;
        IsActive = false;
    }
}
