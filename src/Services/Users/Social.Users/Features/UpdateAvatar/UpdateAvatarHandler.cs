using Microsoft.EntityFrameworkCore;
using Social.Shared.ResultType;
using Social.Users.Application.Errors;
using Social.Users.Infrastructure.Persistence.Context;
using Wolverine.Attributes;

namespace Social.Users.Features.UpdateAvatar;

[Transactional(typeof(UsersDbContext))]
public sealed class UpdateAvatarHandler(
    UsersDbContext context)
{
    public async Task<Result> Handle(
        UpdateAvatarCommand command,
        CancellationToken ct)
    {
        var profile = await context.Profiles
            .FirstOrDefaultAsync(x => 
                x.Id == command.UserId, ct);
        
        if (profile is null)
            return Result.Failure(ProfileErrors.NotFound);

        profile.ImagePath = command.Request.ImagePath;
        profile.UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}