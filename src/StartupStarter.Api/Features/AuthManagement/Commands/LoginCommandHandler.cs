using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Authentication;
using StartupStarter.Api.Features.AuthManagement.Dtos;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;
using StartupStarter.Core.Model.AuthenticationAggregate.Enums;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Features.AuthManagement.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto?>
{
    private readonly IStartupStarterContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(IStartupStarterContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            // Record failed login attempt
            var failedAttempt = new LoginAttempt(
                Guid.NewGuid().ToString(),
                string.Empty,
                request.Email,
                request.IpAddress ?? string.Empty,
                request.UserAgent ?? string.Empty,
                LoginMethod.Password,
                false,
                FailureReason.InvalidCredentials
            );
            _context.LoginAttempts.Add(failedAttempt);
            await _context.SaveChangesAsync(cancellationToken);
            return null;
        }

        // Verify password hash (Note: In production, use proper password hashing like BCrypt)
        if (user.PasswordHash != request.Password) // Simplified - should use BCrypt.Verify
        {
            // Record failed login attempt
            var failedAttempt = new LoginAttempt(
                Guid.NewGuid().ToString(),
                user.UserId,
                request.Email,
                request.IpAddress ?? string.Empty,
                request.UserAgent ?? string.Empty,
                LoginMethod.Password,
                false,
                FailureReason.InvalidCredentials
            );
            _context.LoginAttempts.Add(failedAttempt);
            await _context.SaveChangesAsync(cancellationToken);
            return null;
        }

        // Check if user is active
        if (user.Status != UserStatus.Active)
        {
            var failedAttempt = new LoginAttempt(
                Guid.NewGuid().ToString(),
                user.UserId,
                request.Email,
                request.IpAddress ?? string.Empty,
                request.UserAgent ?? string.Empty,
                LoginMethod.Password,
                false,
                FailureReason.AccountLocked
            );
            _context.LoginAttempts.Add(failedAttempt);
            await _context.SaveChangesAsync(cancellationToken);
            return null;
        }

        // Get user roles
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == user.UserId && ur.IsActive)
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.RoleId,
                (ur, r) => r.RoleName)
            .ToListAsync(cancellationToken);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, userRoles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        // Create user session
        var sessionId = Guid.NewGuid().ToString();
        var session = new UserSession(
            sessionId,
            user.UserId,
            user.AccountId,
            request.IpAddress ?? string.Empty,
            request.UserAgent ?? string.Empty,
            LoginMethod.Password,
            expiresAt
        );
        _context.UserSessions.Add(session);

        // Record successful login attempt
        var successAttempt = new LoginAttempt(
            Guid.NewGuid().ToString(),
            user.UserId,
            request.Email,
            request.IpAddress ?? string.Empty,
            request.UserAgent ?? string.Empty,
            LoginMethod.Password,
            true
        );
        _context.LoginAttempts.Add(successAttempt);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccountId = user.AccountId,
            Roles = userRoles,
            ExpiresAt = expiresAt,
            User =  user.ToDto()
        };
    }
}
