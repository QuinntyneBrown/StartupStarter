using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ProfileManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ProfileManagement.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto?>
{
    private readonly IStartupStarterContext _context;

    public UpdateProfileCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ProfileDto?> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.ProfileId == request.ProfileId, cancellationToken);

        if (profile == null)
            return null;

        var updatedFields = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(request.ProfileName))
        {
            updatedFields["ProfileName"] = request.ProfileName;
        }

        if (!string.IsNullOrEmpty(request.AvatarUrl))
        {
            profile.UpdateAvatar(request.AvatarUrl, request.UpdatedBy);
        }

        if (updatedFields.Count > 0)
        {
            profile.Update(updatedFields, request.UpdatedBy);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return profile.ToDto();
    }
}
