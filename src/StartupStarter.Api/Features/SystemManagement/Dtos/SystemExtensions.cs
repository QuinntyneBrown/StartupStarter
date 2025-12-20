using StartupStarter.Core.Model.MaintenanceAggregate.Entities;
using StartupStarter.Core.Model.BackupAggregate.Entities;
using StartupStarter.Core.Model.SystemErrorAggregate.Entities;

namespace StartupStarter.Api.Features.SystemManagement.Dtos;

public static class SystemExtensions
{
    public static SystemMaintenanceDto ToDto(this SystemMaintenance maintenance)
    {
        var status = "Scheduled";
        if (maintenance.CompletedTime.HasValue)
            status = "Completed";
        else if (maintenance.ActualStartTime.HasValue)
            status = "In Progress";

        return new SystemMaintenanceDto
        {
            MaintenanceId = maintenance.MaintenanceId,
            MaintenanceType = maintenance.MaintenanceType.ToString(),
            Status = status,
            ScheduledAt = maintenance.ScheduledStartTime,
            StartedAt = maintenance.ActualStartTime,
            CompletedAt = maintenance.CompletedTime,
            PerformedBy = string.Empty,
            Notes = string.Empty
        };
    }

    public static SystemBackupDto ToDto(this SystemBackup backup)
    {
        var status = "In Progress";
        if (backup.CompletedAt.HasValue)
            status = backup.Success ? "Completed" : "Failed";

        return new SystemBackupDto
        {
            BackupId = backup.BackupId,
            BackupType = backup.BackupType.ToString(),
            Status = status,
            ScheduledAt = backup.StartedAt,
            StartedAt = backup.StartedAt,
            CompletedAt = backup.CompletedAt,
            BackupSize = backup.BackupSize,
            StorageLocation = backup.BackupLocation
        };
    }

    public static SystemErrorDto ToDto(this SystemError error)
    {
        return new SystemErrorDto
        {
            ErrorId = error.ErrorId,
            ErrorType = error.ErrorType,
            Severity = error.Severity.ToString(),
            Message = error.ErrorMessage,
            StackTrace = error.StackTrace,
            OccurredAt = error.OccurredAt,
            ResolvedAt = null
        };
    }
}
