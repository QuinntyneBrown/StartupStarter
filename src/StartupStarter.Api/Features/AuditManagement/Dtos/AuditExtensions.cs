using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Api.Features.AuditManagement.Dtos;

public static class AuditExtensions
{
    public static AuditLogDto ToDto(this AuditLog auditLog)
    {
        return new AuditLogDto
        {
            LogId = auditLog.AuditId,
            AccountId = auditLog.AccountId,
            UserId = auditLog.PerformedBy,
            Action = auditLog.Action,
            ResourceType = auditLog.EntityType,
            ResourceId = auditLog.EntityId,
            Changes = auditLog.AfterStateJson,
            IpAddress = auditLog.IPAddress,
            UserAgent = string.Empty,
            Timestamp = auditLog.Timestamp
        };
    }

    public static AuditExportDto ToDto(this AuditExport auditExport)
    {
        return new AuditExportDto
        {
            ExportId = auditExport.ExportId,
            AccountId = auditExport.AccountId,
            ExportFormat = auditExport.FileFormat.ToString(),
            Filters = auditExport.FiltersJson,
            Status = auditExport.Status.ToString(),
            RequestedAt = auditExport.RequestedAt,
            RequestedBy = auditExport.RequestedBy,
            CompletedAt = auditExport.CompletedAt,
            DownloadUrl = auditExport.FileLocation
        };
    }

    public static RetentionPolicyDto ToDto(this RetentionPolicy retentionPolicy)
    {
        return new RetentionPolicyDto
        {
            PolicyId = retentionPolicy.RetentionPolicyId,
            AccountId = string.Empty,
            ResourceType = retentionPolicy.PolicyName,
            RetentionDays = retentionPolicy.RetentionDays,
            IsActive = retentionPolicy.IsActive,
            CreatedAt = retentionPolicy.CreatedAt,
            UpdatedAt = retentionPolicy.UpdatedAt
        };
    }
}
