using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.SystemManagement.Queries;

public class GetBackupByIdQueryHandler : IRequestHandler<GetBackupByIdQuery, SystemBackupDto?>
{
    private readonly IStartupStarterContext _context;

    public GetBackupByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<SystemBackupDto?> Handle(GetBackupByIdQuery request, CancellationToken cancellationToken)
    {
        var backup = await _context.SystemBackups
            .FirstOrDefaultAsync(b => b.BackupId == request.BackupId, cancellationToken);

        return backup?.ToDto();
    }
}
