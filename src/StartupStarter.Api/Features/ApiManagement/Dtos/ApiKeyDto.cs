namespace StartupStarter.Api.Features.ApiManagement.Dtos;

public class ApiKeyDto
{
    public string ApiKeyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public DateTime? LastUsedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
