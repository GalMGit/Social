using Microsoft.AspNetCore.Http;

namespace Social.Shared.ResultType;

public static class ResultExtensions
{
    public static IResult ToHttpResponse<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return result.Error!.ToHttpResponse();
    }

    public static IResult ToHttpResponse(
        this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        return result.Error!.ToHttpResponse();
    }

    private static IResult ToHttpResponse(this Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound =>
                Results.NotFound(new
                {
                    error.Code,
                    error.Message
                }),

            ErrorType.Validation =>
                Results.BadRequest(new
                {
                    error.Code,
                    error.Message,
                    error.Detail
                }),

            ErrorType.Conflict =>
                Results.Conflict(new
                {
                    error.Code,
                    error.Message
                }),

            ErrorType.Unauthorized =>
                Results.Unauthorized(),

            ErrorType.Forbidden =>
                Results.Json(
                    new
                    {
                        error.Code,
                        error.Message
                    },
                    statusCode: StatusCodes.Status403Forbidden),
            
            ErrorType.ServiceUnavailable =>
                Results.Json(
                    new
                    {
                        error.Code,
                        error.Message
                    },
                    statusCode: StatusCodes.Status503ServiceUnavailable),

            _ =>
                Results.BadRequest(new
                {
                    error.Code,
                    error.Message
                })
        };
    }
}