using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.AuthenticationManagement.Commands;

public class EndSessionCommandHandler : IRequestHandler<EndSessionCommand, bool>
{
    private readonly IStartupStarterContext _context;

    public EndSessionCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == request.SessionId, cancellationToken);

        if (session == null)
            return false;

        session.EndSession(request.LogoutType);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
