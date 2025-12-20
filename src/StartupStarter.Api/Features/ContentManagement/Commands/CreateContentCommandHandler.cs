using MediatR;
using StartupStarter.Api.Features.ContentManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Api.Features.ContentManagement.Commands;

public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentDto>
{
    private readonly IStartupStarterContext _context;

    public CreateContentCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ContentDto> Handle(CreateContentCommand request, CancellationToken cancellationToken)
    {
        var contentId = Guid.NewGuid().ToString();

        var content = new Content(
            contentId,
            request.ContentType,
            request.Title,
            request.Body,
            request.AuthorId,
            request.AccountId,
            request.ProfileId
        );

        _context.Contents.Add(content);
        await _context.SaveChangesAsync(cancellationToken);

        return content.ToDto();
    }
}
