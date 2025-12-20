using MediatR;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Features.AuthenticationManagement.Commands;

public class CreateSessionCommand : IRequest<UserSessionDto>
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public LoginMethod LoginMethod { get; set; }
    public int? ExpirationMinutes { get; set; }
}
