using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core.Model.BackupAggregate.Enums;

namespace StartupStarter.Api.Features.SystemManagement.Commands;

public class StartBackupCommand : IRequest<SystemBackupDto>
{
    public BackupType BackupType { get; set; }
}
