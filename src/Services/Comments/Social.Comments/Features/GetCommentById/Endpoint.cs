using Social.Comments.Application.DTOs;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Comments.Features.GetCommentById;

public sealed class Endpoint : IEndpoint
{
    public const string Name = "GetCommentById";
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("comments/{commentId:guid}", async (
                Guid commentId,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<
                    Result<CommentResponse>>(
                        new GetCommentByIdQuery(
                            commentId), ct);

                return result.ToHttpResponse();
            })
            .WithTags("Comments")
            .AllowAnonymous()
            .WithName(Name);
    }
}