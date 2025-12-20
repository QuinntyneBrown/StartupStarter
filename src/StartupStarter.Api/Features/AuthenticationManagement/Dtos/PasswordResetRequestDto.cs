namespace StartupStarter.Api.Features.AuthenticationManagement.Dtos;

public class PasswordResetRequestDto
{
    public string RequestId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}
