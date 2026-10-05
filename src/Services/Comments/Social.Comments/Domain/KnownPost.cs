namespace Social.Comments.Domain;

public sealed class KnownPost
{
    public Guid PostId { get; set; }
    public DateTime CreatedAt { get; set; }
}