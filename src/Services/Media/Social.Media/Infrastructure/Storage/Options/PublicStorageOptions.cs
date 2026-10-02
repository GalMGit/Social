namespace Social.Media.Infrastructure.Storage.Options;

public sealed class PublicStorageOptions
{
    public string ServiceUrl { get; init; } = null!;
    public string Bucket { get; init; } = null!;
    public string AccessKey { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public string BaseUrl { get; init; } = null!;
}