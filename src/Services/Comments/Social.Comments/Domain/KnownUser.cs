namespace Social.Comments.Domain;

public sealed class KnownUser
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
}