namespace Social.Posts.Features.CreatePost;

public sealed record CreatePostRequest(
    string Title,
    string Content,
    string? ImagePath,
    Guid? CommunityId);
