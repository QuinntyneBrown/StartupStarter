using MediatR;
using StartupStarter.Api.Features.RoleManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Api.Features.RoleManagement.Commands;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly IStartupStarterContext _context;

    public CreateRoleCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleId = Guid.NewGuid().ToString();

        var role = new Role(
            roleId,
            request.RoleName,
            request.Description,
            request.AccountId,
            request.Permissions,
            request.CreatedBy
        );

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return role.ToDto();
    }
}
