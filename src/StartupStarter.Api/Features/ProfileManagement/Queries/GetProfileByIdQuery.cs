using MediatR;
using StartupStarter.Api.Features.ProfileManagement.Dtos;

namespace StartupStarter.Api.Features.ProfileManagement.Queries;

public class GetProfileByIdQuery : IRequest<ProfileDto?>
{
    public string ProfileId { get; set; } = string.Empty;
}
