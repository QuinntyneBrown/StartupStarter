using MediatR;
using StartupStarter.Api.Features.AuthManagement.Dtos;

namespace StartupStarter.Api.Features.AuthManagement.Commands;

public class LoginCommand : IRequest<LoginDto?>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
