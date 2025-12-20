using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.AuthenticationManagement.Queries;

public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, UserSessionDto?>
{
    private readonly IStartupStarterContext _context;

    public GetSessionByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserSessionDto?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == request.SessionId, cancellationToken);

        return session?.ToDto();
    }
}
