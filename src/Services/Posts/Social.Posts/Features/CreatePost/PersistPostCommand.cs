namespace Social.Posts.Features.CreatePost;

public sealed record PersistPostCommand(
    Guid AuthorId, 
    CreatePostRequest Request);