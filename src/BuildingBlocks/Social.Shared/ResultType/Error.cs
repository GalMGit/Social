namespace Social.Shared.ResultType;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    object? Detail = null)
{
    public static Error Failure(
        string code,
        string message)
        => new(code, message, ErrorType.Failure);

    public static Error Validation(
        string code,
        string message,
        object? detail = null)
        => new(
            code,
            message, 
            ErrorType.Validation,
            detail);

    public static Error NotFound(
        string code,
        string message)
        => new(code, message, ErrorType.NotFound);

    public static Error Conflict(
        string code,
        string message)
        => new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(
        string code,
        string message)
        => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(
        string code,
        string message)
        => new(code, message, ErrorType.Forbidden);
}