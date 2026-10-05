namespace Social.Contracts.Events.Posts;

public sealed record PostDeletedEvent(
    Guid PostId);