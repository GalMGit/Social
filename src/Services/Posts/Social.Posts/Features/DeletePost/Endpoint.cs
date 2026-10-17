using System.Security.Claims;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Posts.Features.DeletePost;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("posts/{postId:guid}", async (
                Guid postId,
                IMessageBus bus,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new DeletePostCommand(
                        postId,
                        user.GetUserId()), ct);

                return result.ToHttpResponse();
            })
            .RequireAuthorization()
            .WithTags("Posts");
    }
}