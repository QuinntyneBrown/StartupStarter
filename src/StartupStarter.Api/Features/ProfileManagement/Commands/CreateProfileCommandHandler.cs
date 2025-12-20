using MediatR;
using StartupStarter.Api.Features.ProfileManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Api.Features.ProfileManagement.Commands;

public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ProfileDto>
{
    private readonly IStartupStarterContext _context;

    public CreateProfileCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ProfileDto> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = Guid.NewGuid().ToString();

        var profile = new Profile(
            profileId,
            request.ProfileName,
            request.AccountId,
            request.CreatedBy,
            request.ProfileType,
            request.IsDefault
        );

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync(cancellationToken);

        return profile.ToDto();
    }
}
