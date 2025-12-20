using MediatR;
using StartupStarter.Api.Features.ProfileManagement.Dtos;
using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Api.Features.ProfileManagement.Commands;

public class CreateProfileCommand : IRequest<ProfileDto>
{
    public string ProfileName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public ProfileType ProfileType { get; set; }
    public bool IsDefault { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
