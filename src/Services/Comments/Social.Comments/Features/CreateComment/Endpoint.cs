using System.Security.Claims;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Social.Shared.Validation;
using Wolverine;

namespace Social.Comments.Features.CreateComment;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("comments", async (
                CreateCommentRequest request,
                ClaimsPrincipal user,
                IMessageBus bus,
                LinkGenerator linkGenerator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result<Guid>>(
                    new CreateCommentCommand(
                        user.GetUserId(),
                        request), ct);

                if (result.IsFailure)
                    return result.ToHttpResponse();

                var location = linkGenerator.GetPathByName(
                    httpContext,
                    GetCommentById.Endpoint.Name,
                    new { commentId = result.Value });

                return Results.Created(location, result.Value);
            })
            .RequireAuthorization()
            .WithTags("Comments")
            .AddEndpointFilter<FluentValidationFilter<CreateCommentRequest>>();
    }
}