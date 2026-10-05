namespace Social.Comments.Domain;

public sealed class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}