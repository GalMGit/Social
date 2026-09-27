using Social.Identity.Domain;

namespace Social.Identity.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string GenerateRefreshToken();
    string GenerateToken(User user);
}