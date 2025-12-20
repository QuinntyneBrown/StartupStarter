using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.RoleManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Features.RoleManagement.Commands;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, UserRoleDto>
{
    private readonly IStartupStarterContext _context;

    public AssignRoleCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserRoleDto> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken);

        if (role == null)
            throw new InvalidOperationException($"Role with ID {request.RoleId} not found");

        var userRoleId = Guid.NewGuid().ToString();

        var userRole = new UserRole(
            userRoleId,
            request.RoleId,
            request.UserId,
            request.AccountId,
            request.AssignedBy
        );

        role.AssignToUser(request.UserId, request.AssignedBy);

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        return userRole.ToDto();
    }
}
