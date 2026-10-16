using System.Security.Claims;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Communities.Features.CreateCommunity;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("communities", async (
                CreateCommunityRequest request,
                ClaimsPrincipal user,
                IMessageBus bus,
                LinkGenerator linkGenerator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result<Guid>>(
                    new CreateCommunityCommand(
                        request, 
                        user.GetUserId()), ct);
                
                if (result.IsFailure)
                    return result.ToHttpResponse();

                var location = linkGenerator.GetPathByName(
                    httpContext,
                    "GetById",
                    new { postId = result.Value });

                return Results.Created(location, result.Value);
            })
            .WithTags("Communities")
            .RequireAuthorization();
    }
}