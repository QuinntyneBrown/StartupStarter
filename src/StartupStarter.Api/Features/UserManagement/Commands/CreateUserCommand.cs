using MediatR;
using StartupStarter.Api.Features.UserManagement.Dtos;

namespace StartupStarter.Api.Features.UserManagement.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<string> InitialRoles { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public bool InvitationSent { get; set; }
}
