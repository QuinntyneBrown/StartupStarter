namespace StartupStarter.Api.Features.UserManagement.Dtos;

public class UserInvitationDto
{
    public string InvitationId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
}
