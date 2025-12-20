using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;

namespace StartupStarter.Api.Features.SystemManagement.Queries;

public class GetBackupByIdQuery : IRequest<SystemBackupDto?>
{
    public string BackupId { get; set; } = string.Empty;
}
