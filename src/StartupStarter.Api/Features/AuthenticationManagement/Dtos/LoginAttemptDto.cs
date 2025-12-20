namespace StartupStarter.Api.Features.AuthenticationManagement.Dtos;

public class LoginAttemptDto
{
    public string AttemptId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime AttemptedAt { get; set; }
    public string? FailureReason { get; set; }
}
