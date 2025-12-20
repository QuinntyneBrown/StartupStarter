using MediatR;
using StartupStarter.Api.Features.ProfileManagement.Dtos;

namespace StartupStarter.Api.Features.ProfileManagement.Queries;

public class GetProfilePreferencesByProfileIdQuery : IRequest<List<ProfilePreferencesDto>>
{
    public string ProfileId { get; set; } = string.Empty;
}
