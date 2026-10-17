using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Communities;
using Social.Posts.Infrastructure.Persistence.Context;
using Wolverine.Attributes;

namespace Social.Posts.IntegrationEvents.Handlers.Communities;

[Transactional(typeof(PostsDbContext))]
public sealed class CommunityDeletedHandler(
    PostsDbContext context)
{
    public async Task Handle(
        CommunityDeletedEvent @event,
        CancellationToken ct)
    {
        var exists = await context.KnownCommunities
            .AsNoTracking()
            .AnyAsync(x =>
                x.CommunityId == @event.CommunityId, ct);
        
        if(!exists)
            return;

        await context.KnownCommunities
            .Where(x =>
                x.CommunityId == @event.CommunityId)
            .ExecuteDeleteAsync(ct);

        await context.Posts
            .Where(x =>
                x.CommunityId == @event.CommunityId)
            .ExecuteDeleteAsync(ct);
    }
}