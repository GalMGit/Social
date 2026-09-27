using Social.Identity.Application.Abstractions.Auth;

namespace Social.Identity.Infrastructure.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
    public string GenerateHash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyHash(
        string password,
        string hashedPassword)
        => BCrypt.Net.BCrypt.Verify(
            password,
            hashedPassword);
}