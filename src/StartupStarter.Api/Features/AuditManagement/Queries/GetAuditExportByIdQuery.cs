using MediatR;
using StartupStarter.Api.Features.AuditManagement.Dtos;

namespace StartupStarter.Api.Features.AuditManagement.Queries;

public class GetAuditExportByIdQuery : IRequest<AuditExportDto?>
{
    public string ExportId { get; set; } = string.Empty;
}
