using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ContentManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ContentManagement.Commands;

public class PublishContentCommandHandler : IRequestHandler<PublishContentCommand, ContentDto>
{
    private readonly IStartupStarterContext _context;

    public PublishContentCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ContentDto> Handle(PublishContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _context.Contents
            .FirstOrDefaultAsync(c => c.ContentId == request.ContentId, cancellationToken);

        if (content == null)
            throw new InvalidOperationException($"Content with ID {request.ContentId} not found");

        content.Publish(request.PublishedBy, request.PublishDate);

        await _context.SaveChangesAsync(cancellationToken);

        return content.ToDto();
    }
}
