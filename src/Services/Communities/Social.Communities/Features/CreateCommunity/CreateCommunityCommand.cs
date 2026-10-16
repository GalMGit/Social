namespace Social.Communities.Features.CreateCommunity;

public sealed record CreateCommunityCommand(
    CreateCommunityRequest Request,
    Guid UserId);