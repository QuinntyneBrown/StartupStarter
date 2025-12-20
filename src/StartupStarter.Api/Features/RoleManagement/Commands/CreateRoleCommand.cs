using MediatR;
using StartupStarter.Api.Features.RoleManagement.Dtos;

namespace StartupStarter.Api.Features.RoleManagement.Commands;

public class CreateRoleCommand : IRequest<RoleDto>
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}
