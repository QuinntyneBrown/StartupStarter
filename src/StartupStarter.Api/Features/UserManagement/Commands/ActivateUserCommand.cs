using MediatR;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Features.UserManagement.Commands;

public class ActivateUserCommand : IRequest<UserDto>
{
    public string UserId { get; set; } = string.Empty;
    public string ActivatedBy { get; set; } = string.Empty;
    public ActivationMethod Method { get; set; }
}
