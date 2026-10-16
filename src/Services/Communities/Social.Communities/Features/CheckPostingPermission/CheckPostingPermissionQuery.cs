namespace Social.Communities.Features.CheckPostingPermission;

public sealed record CheckPostingPermissionQuery(
    Guid CommunityId,
    Guid UserId);