using MediatR;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;

namespace StartupStarter.Api.Features.AuthenticationManagement.Commands;

public class EnableMfaCommandHandler : IRequestHandler<EnableMfaCommand, MfaDto>
{
    private readonly IStartupStarterContext _context;

    public EnableMfaCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<MfaDto> Handle(EnableMfaCommand request, CancellationToken cancellationToken)
    {
        var mfaId = Guid.NewGuid().ToString();

        var mfa = new MultiFactorAuthentication(
            mfaId,
            request.UserId,
            request.AccountId,
            request.Method,
            request.EnabledBy,
            request.SecretKey,
            request.BackupCodesJson
        );

        _context.MultiFactorAuthentications.Add(mfa);
        await _context.SaveChangesAsync(cancellationToken);

        return mfa.ToDto();
    }
}
