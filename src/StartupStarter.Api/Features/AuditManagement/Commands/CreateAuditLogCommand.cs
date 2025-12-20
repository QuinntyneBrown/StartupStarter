using MediatR;
using StartupStarter.Api.Features.AuditManagement.Dtos;

namespace StartupStarter.Api.Features.AuditManagement.Commands;

public class CreateAuditLogCommand : IRequest<AuditLogDto>
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public object? BeforeState { get; set; }
    public object? AfterState { get; set; }
}
