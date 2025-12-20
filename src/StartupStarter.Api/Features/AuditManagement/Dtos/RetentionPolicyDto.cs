namespace StartupStarter.Api.Features.AuditManagement.Dtos;

public class RetentionPolicyDto
{
    public string PolicyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public int RetentionDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
