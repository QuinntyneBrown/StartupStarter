using MediatR;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;

namespace StartupStarter.Api.Features.AuthenticationManagement.Queries;

public class GetLoginAttemptsByUserIdQuery : IRequest<List<LoginAttemptDto>>
{
    public string UserId { get; set; } = string.Empty;
}
