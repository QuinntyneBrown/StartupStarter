using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.UserManagement.Commands;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserDto>
{
    private readonly IStartupStarterContext _context;

    public ActivateUserCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user == null)
            throw new InvalidOperationException($"User with ID {request.UserId} not found");

        user.Activate(request.ActivatedBy, request.Method);
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}
