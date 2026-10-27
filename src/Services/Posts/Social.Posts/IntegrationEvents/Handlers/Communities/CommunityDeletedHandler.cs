using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Social.Contracts.Events.Communities;
using Social.Contracts.Events.Posts;
using Social.Posts.Application.Caching;
using Social.Posts.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.Attributes;

namespace Social.Posts.IntegrationEvents.Handlers.Communities;

[Transactional(typeof(PostsDbContext))]
public sealed class CommunityDeletedHandler(
    PostsDbContext context,
    IMessageBus bus,
    IDistributedCache cache,
    ILogger<CommunityDeletedHandler> logger)
{
    public async Task Handle(
        CommunityDeletedEvent @event,
        CancellationToken ct)
    {
        var posts = await context.Posts
            .AsNoTracking()
            .Where(x => x.CommunityId == @event.CommunityId)
            .Select(x => new PostSnapshot(
                x.Id,
                x.ImagePath,
                x.ThumbnailImagePath))
            .ToListAsync(ct);

        await context.Posts
            .Where(x => x.CommunityId == @event.CommunityId)
            .ExecuteDeleteAsync(ct);

        await context.KnownCommunities
            .Where(x => x.CommunityId == @event.CommunityId)
            .ExecuteDeleteAsync(ct);

        await cache.RemoveAsync(PostCacheKeys.All, ct);

        foreach (var post in posts)
        {
            await cache.RemoveAsync(PostCacheKeys.ById(post.Id), ct);

            var mediaKeys = new[]
                {
                    post.ImagePath, 
                    post.ThumbnailImagePath
                }
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(key => key!)
                .Distinct()
                .ToArray();

            await bus.PublishAsync(
                new PostDeletedEvent(post.Id, mediaKeys));
        }

        logger.LogInformation(
            "Community {CommunityId} removed: {PostCount} posts, {EventCount} PostDeletedEvent(s)",
            @event.CommunityId,
            posts.Count,
            posts.Count);
    }

    private sealed record PostSnapshot(
        Guid Id,
        string? ImagePath,
        string? ThumbnailImagePath);
}