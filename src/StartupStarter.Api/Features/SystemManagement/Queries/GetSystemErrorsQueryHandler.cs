using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.SystemManagement.Queries;

public class GetSystemErrorsQueryHandler : IRequestHandler<GetSystemErrorsQuery, List<SystemErrorDto>>
{
    private readonly IStartupStarterContext _context;

    public GetSystemErrorsQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<SystemErrorDto>> Handle(GetSystemErrorsQuery request, CancellationToken cancellationToken)
    {
        var errors = await _context.SystemErrors
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync(cancellationToken);

        return errors.Select(e => e.ToDto()).ToList();
    }
}
