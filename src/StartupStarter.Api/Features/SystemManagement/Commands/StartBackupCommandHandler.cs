using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.BackupAggregate.Entities;

namespace StartupStarter.Api.Features.SystemManagement.Commands;

public class StartBackupCommandHandler : IRequestHandler<StartBackupCommand, SystemBackupDto>
{
    private readonly IStartupStarterContext _context;

    public StartBackupCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<SystemBackupDto> Handle(StartBackupCommand request, CancellationToken cancellationToken)
    {
        var backupId = Guid.NewGuid().ToString();

        var backup = new SystemBackup(
            backupId,
            request.BackupType
        );

        _context.SystemBackups.Add(backup);
        await _context.SaveChangesAsync(cancellationToken);

        return backup.ToDto();
    }
}
