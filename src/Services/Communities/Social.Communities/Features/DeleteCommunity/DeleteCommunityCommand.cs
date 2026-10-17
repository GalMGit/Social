namespace Social.Communities.Features.DeleteCommunity;

public sealed record DeleteCommunityCommand(
    Guid CommunityId,
    Guid UserId);