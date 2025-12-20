using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.MediaManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.MediaManagement.Queries;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, MediaDto?>
{
    private readonly IStartupStarterContext _context;

    public GetMediaByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<MediaDto?> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        var media = await _context.Medias
            .FirstOrDefaultAsync(m => m.MediaId == request.MediaId, cancellationToken);

        return media?.ToDto();
    }
}
