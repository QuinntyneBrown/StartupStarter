using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ApiManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ApiManagement.Queries;

public class GetApiKeyByIdQueryHandler : IRequestHandler<GetApiKeyByIdQuery, ApiKeyDto?>
{
    private readonly IStartupStarterContext _context;

    public GetApiKeyByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ApiKeyDto?> Handle(GetApiKeyByIdQuery request, CancellationToken cancellationToken)
    {
        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(a => a.ApiKeyId == request.ApiKeyId, cancellationToken);

        return apiKey?.ToDto();
    }
}
