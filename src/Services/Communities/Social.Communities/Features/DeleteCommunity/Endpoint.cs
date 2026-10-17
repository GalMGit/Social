using System.Security.Claims;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Communities.Features.DeleteCommunity;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("communities/{communityId:guid}", async (
                Guid communityId,
                IMessageBus bus,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new DeleteCommunityCommand(
                        communityId,
                        user.GetUserId()), ct);

                return result.ToHttpResponse();
            })
            .RequireAuthorization()
            .WithTags("Communities");
    }
}