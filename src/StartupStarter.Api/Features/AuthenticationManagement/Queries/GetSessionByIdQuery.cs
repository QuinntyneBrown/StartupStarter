using MediatR;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;

namespace StartupStarter.Api.Features.AuthenticationManagement.Queries;

public class GetSessionByIdQuery : IRequest<UserSessionDto?>
{
    public string SessionId { get; set; } = string.Empty;
}
