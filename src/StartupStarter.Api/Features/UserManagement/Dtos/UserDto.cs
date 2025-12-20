namespace StartupStarter.Api.Features.UserManagement.Dtos;

public class UserDto
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public DateTime? LockedAt { get; set; }
    public string? LockReason { get; set; }
}
