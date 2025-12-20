using MediatR;
using StartupStarter.Api.Features.RoleManagement.Dtos;

namespace StartupStarter.Api.Features.RoleManagement.Queries;

public class GetUserRolesByUserIdQuery : IRequest<List<UserRoleDto>>
{
    public string UserId { get; set; } = string.Empty;
}
