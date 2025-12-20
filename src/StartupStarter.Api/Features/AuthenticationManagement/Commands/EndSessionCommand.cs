using MediatR;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Features.AuthenticationManagement.Commands;

public class EndSessionCommand : IRequest<bool>
{
    public string SessionId { get; set; } = string.Empty;
    public LogoutType LogoutType { get; set; }
}
