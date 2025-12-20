using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Features.RoleManagement.Dtos;

public static class RoleExtensions
{
    public static RoleDto ToDto(this Role role)
    {
        return new RoleDto
        {
            RoleId = role.RoleId,
            AccountId = role.AccountId,
            RoleName = role.RoleName,
            Description = role.Description,
            Permissions = string.Join(",", role.Permissions),
            IsSystemRole = false, // Can be enhanced later with logic
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            DeletedAt = role.DeletedAt
        };
    }

    public static UserRoleDto ToDto(this UserRole userRole)
    {
        return new UserRoleDto
        {
            UserRoleId = userRole.UserRoleId,
            UserId = userRole.UserId,
            RoleId = userRole.RoleId,
            AssignedAt = userRole.AssignedAt,
            AssignedBy = userRole.AssignedBy,
            ExpiresAt = null // Can be enhanced later if needed
        };
    }
}
