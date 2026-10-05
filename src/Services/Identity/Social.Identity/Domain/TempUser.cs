namespace Social.Identity.Domain;

public sealed class TempUser
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ConfirmationCode { get; set; }
    public DateTime CodeExpiresAt { get; set; }
    public ICollection<Role> Roles { get; set; } = [];
}