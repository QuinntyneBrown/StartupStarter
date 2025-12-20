using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.RoleManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.RoleManagement.Queries;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto?>
{
    private readonly IStartupStarterContext _context;

    public GetRoleByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken);

        return role?.ToDto();
    }
}
