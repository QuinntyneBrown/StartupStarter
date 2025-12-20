using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Api.Authentication;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    string? ValidateToken(string token);
}
