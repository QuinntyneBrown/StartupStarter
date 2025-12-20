using MediatR;
using StartupStarter.Api.Features.RoleManagement.Dtos;

namespace StartupStarter.Api.Features.RoleManagement.Commands;

public class AssignRoleCommand : IRequest<UserRoleDto>
{
    public string RoleId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
}
