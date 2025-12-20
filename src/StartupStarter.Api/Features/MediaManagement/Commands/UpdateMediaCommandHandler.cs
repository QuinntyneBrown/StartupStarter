using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.MediaManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.MediaManagement.Commands;

public class UpdateMediaCommandHandler : IRequestHandler<UpdateMediaCommand, MediaDto?>
{
    private readonly IStartupStarterContext _context;

    public UpdateMediaCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<MediaDto?> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await _context.Medias
            .FirstOrDefaultAsync(m => m.MediaId == request.MediaId, cancellationToken);

        if (media == null)
            return null;

        media.Update(request.UpdatedFields, request.UpdatedBy);

        await _context.SaveChangesAsync(cancellationToken);

        return media.ToDto();
    }
}
