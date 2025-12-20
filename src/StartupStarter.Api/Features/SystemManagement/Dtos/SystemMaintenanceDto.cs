namespace StartupStarter.Api.Features.SystemManagement.Dtos;

public class SystemMaintenanceDto
{
    public string MaintenanceId { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
