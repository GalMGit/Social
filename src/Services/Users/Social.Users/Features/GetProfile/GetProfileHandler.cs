using Microsoft.EntityFrameworkCore;
using Social.Shared.ResultType;
using Social.Users.Application.DTOs;
using Social.Users.Application.Errors;
using Social.Users.Application.Mappers;
using Social.Users.Application.Media;
using Social.Users.Infrastructure.Persistence.Context;

namespace Social.Users.Features.GetProfile;

public sealed class GetProfileHandler(
    UsersDbContext context,
    IMediaUrlBuilder urlBuilder)
{
    public async Task<Result<ProfileResponse>> Handle(
        GetProfileQuery query,
        CancellationToken ct)
    {
        var profile = await context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == query.UserId, ct);
        
        if(profile is null)
            return Result<ProfileResponse>.Failure(
                ProfileErrors.NotFound);

        var imageUrl = urlBuilder.ToPublicUrl(
            profile.ImagePath);
        
        return Result<ProfileResponse>.Success(
            profile.ToProfileResponse(imageUrl));
    }
}