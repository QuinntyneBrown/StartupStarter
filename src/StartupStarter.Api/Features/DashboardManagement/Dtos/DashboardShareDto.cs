namespace StartupStarter.Api.Features.DashboardManagement.Dtos;

public class DashboardShareDto
{
    public string ShareId { get; set; } = string.Empty;
    public string DashboardId { get; set; } = string.Empty;
    public string SharedWithUserId { get; set; } = string.Empty;
    public string PermissionLevel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
