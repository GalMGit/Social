namespace Social.Media.Infrastructure.Storage.Options;

public sealed class ApiOptions
{
    public string BaseUrl { get; init; } = null!;
    public string ApiPrefix { get; init; } = null!;
}