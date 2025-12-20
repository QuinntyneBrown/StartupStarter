using StartupStarter.Api.Features.UserManagement.Dtos;

namespace StartupStarter.Api.Features.AuthManagement.Dtos;

public class LoginDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
