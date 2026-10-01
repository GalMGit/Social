using System.Security.Claims;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Social.Shared.Validation;
using Wolverine;

namespace Social.Posts.Features.CreatePost;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("posts", async (
                CreatePostRequest request,
                ClaimsPrincipal user,
                IMessageBus bus,
                LinkGenerator linkGenerator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<
                    Result<Guid>>(
                        new CreatePostCommand(
                            user.GetUserId(),
                            request), ct);

                if (result.IsFailure)
                    return result.ToHttpResponse();

                var location = linkGenerator.GetPathByName(
                    httpContext,
                    GetPostById.Endpoint.Name,
                    new { postId = result.Value });

                return Results.Created(location, result.Value);
            })
            .RequireAuthorization()
            .WithTags("Posts")
            .AddEndpointFilter<FluentValidationFilter<CreatePostRequest>>();
    }
}