namespace Social.Comments.Features.CreateComment;

public sealed record CreateCommentCommand(
    Guid AuthorId, 
    CreateCommentRequest Request);