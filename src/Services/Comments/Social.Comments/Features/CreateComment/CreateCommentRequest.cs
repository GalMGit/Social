namespace Social.Comments.Features.CreateComment;

public sealed record CreateCommentRequest(
    Guid PostId, 
    string Text);