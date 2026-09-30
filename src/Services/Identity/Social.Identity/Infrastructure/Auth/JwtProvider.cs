using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Social.Identity.Application.Abstractions.Auth;
using Social.Identity.Domain;
using Social.Shared.Authentication;

namespace Social.Identity.Infrastructure.Auth;

public class JwtProvider(
    IOptions<JwtOptions> options
) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    
    public string GenerateToken(User user)
    {
        List<Claim> claims = [
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
        ];

        claims.AddRange(
            user.Roles.Select(role =>
                new Claim(
                    ClaimTypes.Role,
                    role.Name)));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresHours),
            claims: claims);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}