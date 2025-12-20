using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.RoleManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.RoleManagement.Commands;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    private readonly IStartupStarterContext _context;

    public UpdateRoleCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken);

        if (role == null)
            throw new InvalidOperationException($"Role with ID {request.RoleId} not found");

        var updatedFields = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(request.RoleName))
        {
            updatedFields["RoleName"] = request.RoleName;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            updatedFields["Description"] = request.Description;
        }

        if (updatedFields.Any())
        {
            role.Update(updatedFields, request.UpdatedBy);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return role.ToDto();
    }
}
