using MediatR;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Api.Features.AuthenticationManagement.Commands;

public class EnableMfaCommand : IRequest<MfaDto>
{
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public MfaMethod Method { get; set; }
    public string EnabledBy { get; set; } = string.Empty;
    public string? SecretKey { get; set; }
    public string? BackupCodesJson { get; set; }
}
