using System.Security.Claims;
using Social.Contracts.Http.Communities;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Communities.Features.CheckPostingPermission;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("internal/communities/{communityId:guid}/posting-permission", 
                async (
                    Guid communityId,
                    ClaimsPrincipal user,
                    IMessageBus bus,
                    CancellationToken ct) =>
                {
                    var result = await bus.InvokeAsync<
                        Result<PostingPermissionResponse>>(
                        new CheckPostingPermissionQuery(
                            communityId,
                            user.GetUserId()), ct);

                    return result.ToHttpResponse();
                })
                .RequireAuthorization()
                .ExcludeFromDescription();
    }
}