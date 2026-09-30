using System.Security.Claims;
using Social.Shared.Endpoint;

namespace Social.Identity.Features.Test;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("test", (ClaimsPrincipal user) =>
            {
                return user.GetUserId();
            })
            .RequireAuthorization()
            .WithTags("Identity");
    }
}