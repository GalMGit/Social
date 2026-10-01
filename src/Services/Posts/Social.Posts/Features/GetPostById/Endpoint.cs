using Social.Shared.Endpoint;

namespace Social.Posts.Features.GetPostById;

public sealed class Endpoint : IEndpoint
{
    public const string Name = "GetPostById";
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
    
        app.MapGet("posts/{postId:guid}", () =>
            {

            })
            .WithName(Name);
    }
}