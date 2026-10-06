using Social.Posts.Application.DTOs;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Posts.Features.GetPosts;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("posts", async (
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<
                    Result<List<PostResponse>>>(
                        new GetPostsQuery(), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithTags("Posts");
    }
}