using MediatR;
using StartupStarter.Api.Features.RoleManagement.Dtos;

namespace StartupStarter.Api.Features.RoleManagement.Commands;

public class UpdateRoleCommand : IRequest<RoleDto>
{
    public string RoleId { get; set; } = string.Empty;
    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}
