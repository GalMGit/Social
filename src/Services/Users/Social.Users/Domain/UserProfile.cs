namespace Social.Users.Domain;

public sealed class UserProfile
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? ImagePath { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}