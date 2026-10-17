using Microsoft.EntityFrameworkCore;
using Social.Communities.Application.Errors;
using Social.Communities.Domain;
using Social.Communities.Infrastructure.Persistence.Context;
using Social.Contracts.Events.Communities;
using Social.Shared.ResultType;
using Wolverine;
using Wolverine.Attributes;

namespace Social.Communities.Features.DeleteCommunity;

[Transactional(typeof(CommunitiesDbContext))]
public sealed class DeleteCommunityHandler(
    CommunitiesDbContext context,
    IMessageBus bus)
{
    public async Task<Result> Handle(
        DeleteCommunityCommand command,
        CancellationToken ct)
    {
        var exists = await context.Communities
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == command.CommunityId, ct);

        if (!exists)
            return Result.Failure(
                CommunityErrors.NotFound);

        var role = await context.Members
            .Where(m =>
                m.CommunityId == command.CommunityId &&
                m.UserId == command.UserId)
            .Select(m => (CommunityRole?)m.Role)
            .SingleOrDefaultAsync(ct);

        var canDelete = role is CommunityRole.Owner;

        if (!canDelete)
            return Result.Failure(
                CommunityErrors.AccessDenied);

        await context.Communities
            .Where(x => 
                x.Id == command.CommunityId)
            .ExecuteDeleteAsync(ct);

        await bus.PublishAsync(
            new CommunityDeletedEvent(
                command.CommunityId));

        return Result.Success();
    }
}