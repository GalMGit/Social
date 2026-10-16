using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Communities;
using Social.Posts.Domain;
using Social.Posts.Infrastructure.Persistence.Context;
using Wolverine.Attributes;

namespace Social.Posts.IntegrationEvents.Handlers.Communities;

[Transactional(typeof(PostsDbContext))]
public sealed class CommunityCreatedHandler(
    PostsDbContext context,
    ILogger<CommunityCreatedHandler> logger)
{
    public async Task Handle(
        CommunityCreatedEvent @event,
        CancellationToken ct)
    {
        var exists = await context.KnownCommunities
            .AnyAsync(x =>
                x.CommunityId == @event.CommunityId, ct);
        
        if(exists)
            return;

        await context.KnownCommunities.AddAsync(new KnownCommunity
        {
            CommunityId = @event.CommunityId,
            Name = @event.Name
        }, ct);
        
        logger.LogInformation(
            "KnownCommunity {CommunityId} added", 
            @event.CommunityId);
    }
}