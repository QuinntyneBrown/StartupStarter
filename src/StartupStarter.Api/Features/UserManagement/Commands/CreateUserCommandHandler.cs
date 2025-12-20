using MediatR;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Api.Features.UserManagement.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IStartupStarterContext _context;

    public CreateUserCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid().ToString();

        var user = new User(
            userId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.AccountId,
            request.PasswordHash,
            request.InitialRoles,
            request.CreatedBy,
            request.InvitationSent
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}
