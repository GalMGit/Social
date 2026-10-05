namespace Social.Comments.Application.DTOs;

public sealed record CommentResponse(
    Guid Id, 
    Guid PostId,
    string Text,
    Guid AuthorId, 
    DateTime CreatedAt);