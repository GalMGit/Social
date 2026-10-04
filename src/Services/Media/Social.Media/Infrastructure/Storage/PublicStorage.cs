using Amazon.S3;
using Microsoft.Extensions.Options;
using Social.Media.Application.Abstractions.IServices.IMediaServices;
using Social.Media.Infrastructure.Storage.Options;

namespace Social.Media.Infrastructure.Storage;

public sealed class PublicStorage : S3ObjectStorage, IPublicStorage
{
    private readonly PublicStorageOptions _options;

    public PublicStorage(
        IAmazonS3 client,
        IOptions<PublicStorageOptions> options)
        : base(client, options.Value.Bucket)
    {
        _options = options.Value;
    }
}