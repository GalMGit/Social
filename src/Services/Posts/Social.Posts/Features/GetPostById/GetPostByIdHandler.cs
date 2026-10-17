using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Social.Posts.Application.DTOs;
using Social.Posts.Application.Errors;
using Social.Posts.Application.Media;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;

namespace Social.Posts.Features.GetPostById;

public sealed class GetPostByIdHandler(
    PostsDbContext context,
    IMediaUrlBuilder urlBuilder,
    IDistributedCache cache)
{
    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(5);

    public async Task<Result<PostResponse>> Handle(
        GetPostByIdQuery query,
        CancellationToken ct)
    {
        var cacheKey = $"post:{query.PostId}";

        var cachedJson = await cache.GetStringAsync(
            cacheKey, ct);

        if (cachedJson is not null)
        {
            var cachedPost =
                JsonSerializer.Deserialize<PostResponse>(
                    cachedJson);

            if (cachedPost is not null)
                return Result<PostResponse>.Success(
                    cachedPost);
        }

        var post = await context.Posts
            .AsNoTracking()
            .Where(x => x.Id == query.PostId)
            .Select(x => new PostResponse(
                x.Id,
                x.AuthorId,
                x.Title,
                x.Content,
                x.CommunityId,
                context.KnownCommunities
                    .Where(u => u.CommunityId == x.CommunityId)
                    .Select(u => u.Name)
                    .FirstOrDefault(),
                urlBuilder.ToPublicUrl(x.ImagePath),
                context.KnownUsers
                    .Where(u => u.UserId == x.AuthorId)
                    .Select(u => u.Username)
                    .FirstOrDefault(),
                x.CreatedAt))
            .SingleOrDefaultAsync(ct);

        if(post is null)
            return Result<PostResponse>.Failure(
                PostErrors.NotFound);

        var json = JsonSerializer.Serialize(post);

        await cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    CacheDuration
            },
            ct);

        return Result<PostResponse>.Success(
            post);
    }
}