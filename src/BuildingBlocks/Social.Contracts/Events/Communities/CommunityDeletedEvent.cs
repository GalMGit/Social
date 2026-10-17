namespace Social.Contracts.Events.Communities;

public sealed record CommunityDeletedEvent(
    Guid CommunityId);