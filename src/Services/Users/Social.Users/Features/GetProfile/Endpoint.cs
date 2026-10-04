using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Social.Users.Application.DTOs;
using Wolverine;

namespace Social.Users.Features.GetProfile;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId}/profile", async (
                Guid userId,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<
                    Result<ProfileResponse>>(
                        new GetProfileQuery(
                            userId), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithTags("Users");
    }
}