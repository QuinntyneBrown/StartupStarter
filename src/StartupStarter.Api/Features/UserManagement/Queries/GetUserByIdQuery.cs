using MediatR;
using StartupStarter.Api.Features.UserManagement.Dtos;

namespace StartupStarter.Api.Features.UserManagement.Queries;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public string UserId { get; set; } = string.Empty;
}
