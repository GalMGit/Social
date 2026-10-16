using Microsoft.EntityFrameworkCore;
using Social.Communities.Application.Errors;
using Social.Communities.Domain;
using Social.Communities.Infrastructure.Persistence.Context;
using Social.Contracts.Http.Communities;
using Social.Shared.ResultType;

namespace Social.Communities.Features.CheckPostingPermission;

public sealed class CheckPostingPermissionHandler(
    CommunitiesDbContext context)
{
    public async Task<Result<PostingPermissionResponse>> Handle(
        CheckPostingPermissionQuery query,
        CancellationToken ct)
    {
        var communityExists = await context.Communities
            .AnyAsync(c => c.Id == query.CommunityId, ct);

        if (!communityExists)
            return Result<PostingPermissionResponse>.Failure(
                CommunityErrors.NotFound);

        var role = await context.Members
            .Where(m =>
                m.CommunityId == query.CommunityId &&
                m.UserId == query.UserId)
            .Select(m => (CommunityRole?)m.Role)
            .SingleOrDefaultAsync(ct);

        var canCreatePost = role is
            CommunityRole.Owner or
            CommunityRole.Admin or
            CommunityRole.Moderator;

        return Result<PostingPermissionResponse>.Success(
            new PostingPermissionResponse(
                canCreatePost));
    }
}