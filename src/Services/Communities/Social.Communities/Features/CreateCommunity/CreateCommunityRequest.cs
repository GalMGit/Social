namespace Social.Communities.Features.CreateCommunity;

public sealed record CreateCommunityRequest(
    string Name,
    string? Description);