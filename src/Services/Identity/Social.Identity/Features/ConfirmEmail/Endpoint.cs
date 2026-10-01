using Social.Shared.Endpoint;
using Social.Shared.ResultType;
using Social.Shared.Validation;
using Wolverine;

namespace Social.Identity.Features.ConfirmEmail;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/email/confirm", async (
                ConfirmEmailRequest request,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new ConfirmEmailCommand(
                        request), ct);

                return result.ToHttpResponse();
            })
            .WithTags("Identity")
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidationFilter<ConfirmEmailRequest>>();
    }
}