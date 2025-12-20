using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.MediaManagement.Commands;

public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, bool>
{
    private readonly IStartupStarterContext _context;

    public DeleteMediaCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await _context.Medias
            .FirstOrDefaultAsync(m => m.MediaId == request.MediaId, cancellationToken);

        if (media == null)
            return false;

        media.Delete(request.DeletedBy, request.DeletionType);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
