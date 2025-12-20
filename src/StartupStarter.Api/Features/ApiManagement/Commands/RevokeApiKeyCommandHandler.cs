using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ApiManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ApiManagement.Commands;

public class RevokeApiKeyCommandHandler : IRequestHandler<RevokeApiKeyCommand, ApiKeyDto?>
{
    private readonly IStartupStarterContext _context;

    public RevokeApiKeyCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ApiKeyDto?> Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
    {
        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(a => a.ApiKeyId == request.ApiKeyId, cancellationToken);

        if (apiKey == null)
            return null;

        apiKey.Revoke(request.RevokedBy, request.Reason);
        await _context.SaveChangesAsync(cancellationToken);

        return apiKey.ToDto();
    }
}
