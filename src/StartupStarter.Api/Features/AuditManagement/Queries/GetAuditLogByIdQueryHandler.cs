using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.AuditManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.AuditManagement.Queries;

public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
{
    private readonly IStartupStarterContext _context;

    public GetAuditLogByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        var auditLog = await _context.AuditLogs
            .FirstOrDefaultAsync(a => a.AuditId == request.AuditId, cancellationToken);

        return auditLog?.ToDto();
    }
}
