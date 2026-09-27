using Microsoft.AspNetCore.Routing;

namespace Social.Shared.Endpoint;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}