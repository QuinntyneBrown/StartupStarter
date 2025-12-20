using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ProfileManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ProfileManagement.Queries;

public class GetProfilePreferencesByProfileIdQueryHandler : IRequestHandler<GetProfilePreferencesByProfileIdQuery, List<ProfilePreferencesDto>>
{
    private readonly IStartupStarterContext _context;

    public GetProfilePreferencesByProfileIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<ProfilePreferencesDto>> Handle(GetProfilePreferencesByProfileIdQuery request, CancellationToken cancellationToken)
    {
        var preferences = await _context.ProfilePreferences
            .Where(p => p.ProfileId == request.ProfileId)
            .ToListAsync(cancellationToken);

        return preferences.Select(p => p.ToDto()).ToList();
    }
}
