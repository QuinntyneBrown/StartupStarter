namespace StartupStarter.Api.Features.SystemManagement.Dtos;

public class SystemErrorDto
{
    public string ErrorId { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
