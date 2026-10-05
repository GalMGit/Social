namespace Social.Identity.Domain;

public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? BannedAt { get; set; }

    public ICollection<Role> Roles { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}