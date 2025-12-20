namespace StartupStarter.Api.Features.AuthenticationManagement.Dtos;

public class MfaDto
{
    public string MfaId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string MfaType { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string BackupCodes { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime EnabledAt { get; set; }
    public DateTime? DisabledAt { get; set; }
}
