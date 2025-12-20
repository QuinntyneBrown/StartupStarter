using MediatR;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;

namespace StartupStarter.Api.Features.AuthenticationManagement.Commands;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, UserSessionDto>
{
    private readonly IStartupStarterContext _context;

    public CreateSessionCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<UserSessionDto> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionId = Guid.NewGuid().ToString();
        DateTime? expiresAt = request.ExpirationMinutes.HasValue
            ? DateTime.UtcNow.AddMinutes(request.ExpirationMinutes.Value)
            : null;

        var session = new UserSession(
            sessionId,
            request.UserId,
            request.AccountId,
            request.IpAddress,
            request.UserAgent,
            request.LoginMethod,
            expiresAt
        );

        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return session.ToDto();
    }
}
