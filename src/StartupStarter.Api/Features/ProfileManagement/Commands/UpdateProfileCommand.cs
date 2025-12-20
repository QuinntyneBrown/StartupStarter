using MediatR;
using StartupStarter.Api.Features.ProfileManagement.Dtos;

namespace StartupStarter.Api.Features.ProfileManagement.Commands;

public class UpdateProfileCommand : IRequest<ProfileDto?>
{
    public string ProfileId { get; set; } = string.Empty;
    public string? ProfileName { get; set; }
    public string? AvatarUrl { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}
