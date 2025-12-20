using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.AuthenticationManagement.Queries;

public class GetLoginAttemptsByUserIdQueryHandler : IRequestHandler<GetLoginAttemptsByUserIdQuery, List<LoginAttemptDto>>
{
    private readonly IStartupStarterContext _context;

    public GetLoginAttemptsByUserIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<LoginAttemptDto>> Handle(GetLoginAttemptsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var attempts = await _context.LoginAttempts
            .Where(a => a.UserId == request.UserId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);

        return attempts.Select(a => a.ToDto()).ToList();
    }
}
