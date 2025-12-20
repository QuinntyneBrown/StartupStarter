namespace StartupStarter.Api.Features.ProfileManagement.Dtos;

public class ProfileShareDto
{
    public string ShareId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string SharedWithUserId { get; set; } = string.Empty;
    public string PermissionLevel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
