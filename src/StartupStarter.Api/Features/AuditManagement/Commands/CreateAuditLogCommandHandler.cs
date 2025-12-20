using MediatR;
using StartupStarter.Api.Features.AuditManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Api.Features.AuditManagement.Commands;

public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, AuditLogDto>
{
    private readonly IStartupStarterContext _context;

    public CreateAuditLogCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<AuditLogDto> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditLog = AuditLog.Create(
            request.EntityType,
            request.EntityId,
            request.AccountId,
            request.Action,
            request.PerformedBy,
            request.IPAddress,
            request.BeforeState ?? new { },
            request.AfterState ?? new { }
        );

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);

        return auditLog.ToDto();
    }
}
