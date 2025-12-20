namespace StartupStarter.Api.Features.RoleManagement.Dtos;

public class RoleDto
{
    public string RoleId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
