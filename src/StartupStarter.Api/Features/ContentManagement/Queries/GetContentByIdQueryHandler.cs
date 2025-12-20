using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ContentManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ContentManagement.Queries;

public class GetContentByIdQueryHandler : IRequestHandler<GetContentByIdQuery, ContentDto?>
{
    private readonly IStartupStarterContext _context;

    public GetContentByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ContentDto?> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
    {
        var content = await _context.Contents
            .FirstOrDefaultAsync(c => c.ContentId == request.ContentId, cancellationToken);

        return content?.ToDto();
    }
}
