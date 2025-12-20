using MediatR;
using StartupStarter.Api.Features.UserManagement.Dtos;

namespace StartupStarter.Api.Features.UserManagement.Queries;

public class GetUserInvitationByIdQuery : IRequest<UserInvitationDto?>
{
    public string InvitationId { get; set; } = string.Empty;
}
