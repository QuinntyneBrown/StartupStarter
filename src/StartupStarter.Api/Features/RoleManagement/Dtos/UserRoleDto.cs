namespace StartupStarter.Api.Features.RoleManagement.Dtos;

public class UserRoleDto
{
    public string UserRoleId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string AssignedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}
