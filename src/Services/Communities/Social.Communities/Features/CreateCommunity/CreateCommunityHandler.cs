using Social.Communities.Domain;
using Social.Communities.Infrastructure.Persistence.Context;
using Social.Contracts.Events.Communities;
using Social.Shared.ResultType;
using Wolverine;
using Wolverine.Attributes;

namespace Social.Communities.Features.CreateCommunity;

[Transactional(typeof(CommunitiesDbContext))]
public sealed class CreateCommunityHandler(
    CommunitiesDbContext context,
    IMessageBus bus)
{
    public async Task<Result<Guid>> Handle(
        CreateCommunityCommand command, 
        CancellationToken ct)
    {
        var community = new Community
        {
            Id = Guid.CreateVersion7(),
            Name = command.Request.Name,
            OwnerId = command.UserId,
            Description = command.Request.Description,
            CreatedAt = DateTime.UtcNow
        };

        community.Members.Add(new CommunityMember
        {
            Id = Guid.CreateVersion7(),
            CommunityId = community.Id,
            Role = CommunityRole.Owner,
            UserId = command.UserId
        });

        await context.AddAsync(community, ct);

        await bus.PublishAsync(
            new CommunityCreatedEvent(
                community.Id,
                community.Name));

        return Result<Guid>.Success(
            community.Id);
    }
}