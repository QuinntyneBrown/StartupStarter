using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.UserManagement.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IStartupStarterContext _context;

    public GetUserByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        return user?.ToDto();
    }
}
