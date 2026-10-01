using Social.Posts.Application.DTOs;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Posts.Features.GetPostById;

public sealed class Endpoint : IEndpoint
{
    public const string Name = "GetPostById";
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
    
        app.MapGet("posts/{postId:guid}", async (
                Guid postId,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<
                    Result<PostResponse>>(
                        new GetPostByIdQuery(
                            postId), ct);

                return result.ToHttpResponse();
            })
            .WithTags("Posts")
            .AllowAnonymous()
            .WithName(Name);
    }
}