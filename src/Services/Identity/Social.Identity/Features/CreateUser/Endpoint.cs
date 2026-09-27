using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Social.Shared.Validation;
using Wolverine;

namespace Social.Identity.Features.CreateUser;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
                CreateUserRequest request,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new CreateUserCommand(
                        request), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithTags("Identity")
            .AddEndpointFilter<FluentValidationFilter<CreateUserRequest>>();
    }
}