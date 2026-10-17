namespace Social.Posts.Features.DeletePost;

public sealed record DeletePostCommand(
    Guid PostId,
    Guid UserId);