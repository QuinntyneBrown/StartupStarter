using MediatR;
using StartupStarter.Api.Features.AuditManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Api.Features.AuditManagement.Commands;

public class RequestExportCommandHandler : IRequestHandler<RequestExportCommand, AuditExportDto>
{
    private readonly IStartupStarterContext _context;

    public RequestExportCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<AuditExportDto> Handle(RequestExportCommand request, CancellationToken cancellationToken)
    {
        var exportId = Guid.NewGuid().ToString();

        var auditExport = new AuditExport(
            exportId,
            request.AccountId,
            request.RequestedBy,
            request.StartDate,
            request.EndDate,
            request.Filters,
            request.FileFormat
        );

        _context.AuditExports.Add(auditExport);
        await _context.SaveChangesAsync(cancellationToken);

        return auditExport.ToDto();
    }
}
