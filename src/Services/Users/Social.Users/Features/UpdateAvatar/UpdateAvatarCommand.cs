namespace Social.Users.Features.UpdateAvatar;

public sealed record UpdateAvatarCommand(
    UpdateAvatarRequest Request,
    Guid UserId);