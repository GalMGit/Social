namespace Social.Posts.Features.CreatePost;

public sealed record CreatePostCommand(
    Guid AuthorId,
    CreatePostRequest Request);