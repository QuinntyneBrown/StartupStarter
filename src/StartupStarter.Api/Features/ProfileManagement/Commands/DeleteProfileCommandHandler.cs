using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ProfileManagement.Commands;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, bool>
{
    private readonly IStartupStarterContext _context;

    public DeleteProfileCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.ProfileId == request.ProfileId, cancellationToken);

        if (profile == null)
            return false;

        profile.Delete(request.DeletedBy);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
