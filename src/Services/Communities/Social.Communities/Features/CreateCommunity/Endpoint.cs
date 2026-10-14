using Social.Shared.Endpoint;

namespace Social.Communities.Features.CreateCommunity;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("communities", async (
                ) =>
                {
                    return "1";
                })
            .WithTags("Communities")
            .RequireAuthorization();
    }
}