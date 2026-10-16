namespace Social.Contracts.Http.Common;

public sealed record ErrorResponse(
    string Code,
    string Message);