using MediatR;
using StartupStarter.Api.Features.RoleManagement.Dtos;

namespace StartupStarter.Api.Features.RoleManagement.Queries;

public class GetRoleByIdQuery : IRequest<RoleDto?>
{
    public string RoleId { get; set; } = string.Empty;
}
