namespace StartupStarter.Api.Features.DashboardManagement.Dtos;

public class DashboardDto
{
    public string DashboardId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string DashboardName { get; set; } = string.Empty;
    public string Layout { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
