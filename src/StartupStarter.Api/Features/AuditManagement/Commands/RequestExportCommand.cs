using MediatR;
using StartupStarter.Api.Features.AuditManagement.Dtos;
using StartupStarter.Core.Model.AuditAggregate.Enums;

namespace StartupStarter.Api.Features.AuditManagement.Commands;

public class RequestExportCommand : IRequest<AuditExportDto>
{
    public string AccountId { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new();
    public FileFormat FileFormat { get; set; }
}
