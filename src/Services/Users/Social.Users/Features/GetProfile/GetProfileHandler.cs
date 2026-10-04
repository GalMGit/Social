using Microsoft.EntityFrameworkCore;
using Social.Shared.ResultType;
using Social.Users.Application.DTOs;
using Social.Users.Application.Errors;
using Social.Users.Application.Mappers;
using Social.Users.Infrastructure.Persistence.Context;

namespace Social.Users.Features.GetProfile;

public sealed class GetProfileHandler(
    UsersDbContext context)
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

        return Result<ProfileResponse>.Success(
            profile.ToProfileResponse());
    }
}