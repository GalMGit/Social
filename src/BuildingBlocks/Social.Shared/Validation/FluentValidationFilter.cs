using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Social.Shared.ResultType;

namespace Social.Shared.Validation;

public sealed class FluentValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices
            .GetService<IValidator<T>>();

        if (validator is null)
            return await next(context);

        var model = context.Arguments
            .OfType<T>()
            .FirstOrDefault();

        if (model is null)
            return await next(context);

        var validationResult = await validator.ValidateAsync(
            model,
            context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
            return await next(context);

        var errors = validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => char.ToLowerInvariant(x.Key[0]) + x.Key[1..],
                x => x.Select(e => e.ErrorMessage).ToArray());

        return Result.Failure(
                Error.Validation(
                    "validation.failed",
                    "One or more validation errors occurred.",
                    errors))
            .ToHttpResponse();
    }
}