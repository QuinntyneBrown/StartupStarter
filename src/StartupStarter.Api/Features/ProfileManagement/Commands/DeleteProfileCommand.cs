using MediatR;

namespace StartupStarter.Api.Features.ProfileManagement.Commands;

public class DeleteProfileCommand : IRequest<bool>
{
    public string ProfileId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
}
