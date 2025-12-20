using StartupStarter.Core.Model.RoleAggregate.Events;

namespace StartupStarter.Core.Model.RoleAggregate.Entities;

public class Role
{
    public string RoleId { get; private set; }
    public string RoleName { get; private set; }
    public string Description { get; private set; }
    public string AccountId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private readonly List<string> _permissions = new();
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private Role()
    {
        RoleId = string.Empty;
        RoleName = string.Empty;
        Description = string.Empty;
        AccountId = string.Empty;
    }

    public Role(string roleId, string roleName, string description, string accountId,
        List<string> permissions, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty", nameof(roleName));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));

        RoleId = roleId;
        RoleName = roleName;
        Description = description ?? string.Empty;
        AccountId = accountId;
        CreatedAt = DateTime.UtcNow;

        if (permissions != null && permissions.Any())
        {
            _permissions.AddRange(permissions);
        }

        AddDomainEvent(new RoleCreatedEvent
        {
            RoleId = RoleId,
            RoleName = RoleName,
            Description = Description,
            AccountId = AccountId,
            Permissions = permissions ?? new List<string>(),
            CreatedBy = createdBy,
            Timestamp = CreatedAt
        });
    }

    public void Update(Dictionary<string, object> updatedFields, string updatedBy)
    {
        if (updatedFields == null || !updatedFields.Any())
            throw new ArgumentException("Updated fields cannot be empty", nameof(updatedFields));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be empty", nameof(updatedBy));

        var previousValues = new Dictionary<string, object>();

        if (updatedFields.ContainsKey("RoleName"))
        {
            previousValues["RoleName"] = RoleName;
            RoleName = updatedFields["RoleName"].ToString() ?? RoleName;
        }

        if (updatedFields.ContainsKey("Description"))
        {
            previousValues["Description"] = Description;
            Description = updatedFields["Description"].ToString() ?? Description;
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new RoleUpdatedEvent
        {
            RoleId = RoleId,
            RoleName = RoleName,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            PreviousValues = previousValues,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy, int affectedUserCount)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy cannot be empty", nameof(deletedBy));

        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new RoleDeletedEvent
        {
            RoleId = RoleId,
            RoleName = RoleName,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            AffectedUserCount = affectedUserCount,
            Timestamp = UpdatedAt.Value
        });
    }

    public void AssignToUser(string userId, string assignedBy)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(assignedBy))
            throw new ArgumentException("AssignedBy cannot be empty", nameof(assignedBy));

        AddDomainEvent(new RoleAssignedToUserEvent
        {
            RoleId = RoleId,
            RoleName = RoleName,
            UserId = userId,
            AccountId = AccountId,
            AssignedBy = assignedBy,
            Timestamp = DateTime.UtcNow
        });
    }

    public void RevokeFromUser(string userId, string revokedBy, string reason)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(revokedBy))
            throw new ArgumentException("RevokedBy cannot be empty", nameof(revokedBy));

        AddDomainEvent(new RoleRevokedFromUserEvent
        {
            RoleId = RoleId,
            RoleName = RoleName,
            UserId = userId,
            AccountId = AccountId,
            RevokedBy = revokedBy,
            Reason = reason ?? string.Empty,
            Timestamp = DateTime.UtcNow
        });
    }

    public void UpdatePermissions(List<string> addedPermissions, List<string> removedPermissions, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be empty", nameof(updatedBy));

        if (addedPermissions != null && addedPermissions.Any())
        {
            foreach (var permission in addedPermissions.Where(p => !_permissions.Contains(p)))
            {
                _permissions.Add(permission);
            }
        }

        if (removedPermissions != null && removedPermissions.Any())
        {
            foreach (var permission in removedPermissions)
            {
                _permissions.Remove(permission);
            }
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new RolePermissionsUpdatedEvent
        {
            RoleId = RoleId,
            RoleName = RoleName,
            AccountId = AccountId,
            AddedPermissions = addedPermissions ?? new List<string>(),
            RemovedPermissions = removedPermissions ?? new List<string>(),
            UpdatedBy = updatedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
