using Social.Comments.Application.DTOs;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Comments.Features.GetCommentsByPost;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("comments/post/{postId}", async (
                Guid postId,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result<
                    List<CommentResponse>>>(
                        new GetCommentsByPostQuery(
                            postId), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithTags("Comments");
    }
}