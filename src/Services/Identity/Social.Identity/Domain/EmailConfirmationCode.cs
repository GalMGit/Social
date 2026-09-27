namespace Social.Identity.Domain;

public sealed class EmailConfirmationCode
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public User User { get; set; }
}