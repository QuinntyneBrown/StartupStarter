using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.AuditManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.AuditManagement.Queries;

public class GetAuditExportByIdQueryHandler : IRequestHandler<GetAuditExportByIdQuery, AuditExportDto?>
{
    private readonly IStartupStarterContext _context;

    public GetAuditExportByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<AuditExportDto?> Handle(GetAuditExportByIdQuery request, CancellationToken cancellationToken)
    {
        var auditExport = await _context.AuditExports
            .FirstOrDefaultAsync(a => a.ExportId == request.ExportId, cancellationToken);

        return auditExport?.ToDto();
    }
}
