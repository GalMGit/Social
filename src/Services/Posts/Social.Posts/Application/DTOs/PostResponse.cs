namespace Social.Posts.Application.DTOs;

public sealed record PostResponse(
    Guid Id, 
    Guid AuthorId,
    string Title, 
    string Content,
    Guid? CommunityId,
    string? ImageUrl,
    DateTime CreatedAt);