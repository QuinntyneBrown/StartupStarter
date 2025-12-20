using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.ApiManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.ApiManagement.Queries;

public class GetApiRequestsByApiKeyIdQueryHandler : IRequestHandler<GetApiRequestsByApiKeyIdQuery, List<ApiRequestDto>>
{
    private readonly IStartupStarterContext _context;

    public GetApiRequestsByApiKeyIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<ApiRequestDto>> Handle(GetApiRequestsByApiKeyIdQuery request, CancellationToken cancellationToken)
    {
        var apiRequests = await _context.ApiRequests
            .Where(r => r.ApiKeyId == request.ApiKeyId)
            .OrderByDescending(r => r.Timestamp)
            .ToListAsync(cancellationToken);

        return apiRequests.Select(r => r.ToDto()).ToList();
    }
}
