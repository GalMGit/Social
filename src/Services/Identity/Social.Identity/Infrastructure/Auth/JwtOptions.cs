namespace Social.Identity.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int Expires { get; set; }
}