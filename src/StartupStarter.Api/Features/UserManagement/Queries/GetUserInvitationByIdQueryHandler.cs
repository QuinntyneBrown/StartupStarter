using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.UserManagement.Queries;

public class GetUserInvitationByIdQueryHandler : IRequestHandler<GetUserInvitationByIdQuery, UserInvitationDto?>
{
    private readonly IStartupStarterContext _context;

    public GetUserInvitationByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserInvitationDto?> Handle(GetUserInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        var invitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.InvitationId == request.InvitationId, cancellationToken);

        return invitation?.ToDto();
    }
}
