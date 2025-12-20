using MediatR;
using StartupStarter.Api.Features.AuditManagement.Dtos;

namespace StartupStarter.Api.Features.AuditManagement.Queries;

public class GetAuditLogByIdQuery : IRequest<AuditLogDto?>
{
    public string AuditId { get; set; } = string.Empty;
}
