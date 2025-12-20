namespace StartupStarter.Api.Features.SystemManagement.Dtos;

public class SystemBackupDto
{
    public string BackupId { get; set; } = string.Empty;
    public string BackupType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? BackupSize { get; set; }
    public string StorageLocation { get; set; } = string.Empty;
}
