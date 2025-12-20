namespace StartupStarter.Api.Features.AuditManagement.Dtos;

public class AuditExportDto
{
    public string ExportId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ExportFormat { get; set; } = string.Empty;
    public string Filters { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}
