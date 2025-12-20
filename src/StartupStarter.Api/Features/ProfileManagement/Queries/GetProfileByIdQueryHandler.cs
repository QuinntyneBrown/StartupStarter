using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ProfileManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ProfileManagement.Queries;

public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto?>
{
    private readonly IStartupStarterContext _context;

    public GetProfileByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ProfileDto?> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.ProfileId == request.ProfileId, cancellationToken);

        return profile?.ToDto();
    }
}
