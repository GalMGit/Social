using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Identity.Features.Login;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
                LoginRequest request,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<
                    Result<LoginResponse>>(
                        new LoginCommand(
                            request), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithTags("Identity");
    }
}