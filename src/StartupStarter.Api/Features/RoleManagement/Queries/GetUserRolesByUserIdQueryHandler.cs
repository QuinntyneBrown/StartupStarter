using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.RoleManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.RoleManagement.Queries;

public class GetUserRolesByUserIdQueryHandler : IRequestHandler<GetUserRolesByUserIdQuery, List<UserRoleDto>>
{
    private readonly IStartupStarterContext _context;

    public GetUserRolesByUserIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<UserRoleDto>> Handle(GetUserRolesByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == request.UserId && ur.IsActive)
            .ToListAsync(cancellationToken);

        return userRoles.Select(ur => ur.ToDto()).ToList();
    }
}
