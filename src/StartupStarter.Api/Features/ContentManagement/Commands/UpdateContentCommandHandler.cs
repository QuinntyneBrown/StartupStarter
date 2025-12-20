using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ContentManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ContentManagement.Commands;

public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, ContentDto>
{
    private readonly IStartupStarterContext _context;

    public UpdateContentCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ContentDto> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _context.Contents
            .FirstOrDefaultAsync(c => c.ContentId == request.ContentId, cancellationToken);

        if (content == null)
            throw new InvalidOperationException($"Content with ID {request.ContentId} not found");

        var updatedFields = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(request.Title))
            updatedFields["Title"] = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Body))
            updatedFields["Body"] = request.Body;

        content.Update(updatedFields, request.UpdatedBy);

        await _context.SaveChangesAsync(cancellationToken);

        return content.ToDto();
    }
}
