using MediatR;
using StartupStarter.Api.Features.MediaManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.MediaAggregate.Entities;

namespace StartupStarter.Api.Features.MediaManagement.Commands;

public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, MediaDto>
{
    private readonly IStartupStarterContext _context;

    public UploadMediaCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<MediaDto> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        var mediaId = Guid.NewGuid().ToString();

        var media = new Media(
            mediaId,
            request.FileName,
            request.FileType,
            request.FileSize,
            request.UploadedBy,
            request.AccountId,
            request.ProfileId,
            request.StorageLocation
        );

        _context.Medias.Add(media);
        await _context.SaveChangesAsync(cancellationToken);

        return media.ToDto();
    }
}
