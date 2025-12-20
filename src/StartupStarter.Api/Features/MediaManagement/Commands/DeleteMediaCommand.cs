using MediatR;
using StartupStarter.Core.Model.MediaAggregate.Enums;

namespace StartupStarter.Api.Features.MediaManagement.Commands;

public class DeleteMediaCommand : IRequest<bool>
{
    public string MediaId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DeletionType DeletionType { get; set; }
}
