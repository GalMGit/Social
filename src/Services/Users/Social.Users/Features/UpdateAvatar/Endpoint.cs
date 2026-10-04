using System.Security.Claims;
using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Users.Features.UpdateAvatar;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/me/avatar", async (
                UpdateAvatarRequest request,
                ClaimsPrincipal user,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new UpdateAvatarCommand(
                        request,
                        user.GetUserId()), ct);

                return result.ToHttpResponse();
            })
            .WithTags("Users")
            .RequireAuthorization();
    }
}