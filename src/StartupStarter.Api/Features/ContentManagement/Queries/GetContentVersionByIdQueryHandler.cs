using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ContentManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ContentManagement.Queries;

public class GetContentVersionByIdQueryHandler : IRequestHandler<GetContentVersionByIdQuery, ContentVersionDto?>
{
    private readonly IStartupStarterContext _context;

    public GetContentVersionByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ContentVersionDto?> Handle(GetContentVersionByIdQuery request, CancellationToken cancellationToken)
    {
        var contentVersion = await _context.ContentVersions
            .FirstOrDefaultAsync(cv => cv.ContentId == request.ContentId && cv.ContentVersionId == request.VersionId, cancellationToken);

        return contentVersion?.ToDto();
    }
}
