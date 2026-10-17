
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Social.Posts.Application.DTOs;
using Social.Posts.Application.Media;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;

namespace Social.Posts.Features.GetPosts;

public sealed class GetPostsHandler(
    PostsDbContext context,
    IMediaUrlBuilder urlBuilder,
    IDistributedCache cache)
{
    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(1);

    public async Task<Result<List<PostResponse>>> Handle(
        GetPostsQuery query,
        CancellationToken ct)
    {
        const string cacheKey = "posts:all";
        var cachedJson = await cache.GetStringAsync(
            cacheKey, ct);

        if (cachedJson is not null)
        {
            var cachedPosts =
                JsonSerializer.Deserialize<List<PostResponse>>(
                    cachedJson);

            if (cachedPosts is not null)
                return Result<List<PostResponse>>.Success(
                    cachedPosts);
        }

        var posts = await context.Posts
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
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
            .ToListAsync(ct);

        var json = JsonSerializer.Serialize(posts);

        await cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    CacheDuration
            },
            ct);

        return Result<List<PostResponse>>.Success(posts);
    }
}
