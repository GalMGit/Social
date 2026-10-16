namespace Social.Contracts.Events.Communities;

public sealed record CommunityCreatedEvent(
    Guid CommunityId,
    string Name);