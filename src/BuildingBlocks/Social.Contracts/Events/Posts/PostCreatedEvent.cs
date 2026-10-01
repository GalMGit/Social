namespace Social.Contracts.Events.Posts;

public sealed record PostCreatedEvent(
    Guid PostId,
    Guid AuthorId,
    string Title,
    string Content,
    Guid? CommunityId);
