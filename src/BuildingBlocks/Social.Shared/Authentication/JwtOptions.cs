namespace Social.Shared.Authentication;

public sealed class JwtOptions
{
    public string SecretKey { get; set; }
    public int Expires { get; set; }
}