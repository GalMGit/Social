using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Options;
using Social.Media.Application.Abstractions.IServices.IMediaServices;
using Social.Media.Infrastructure.Storage;
using Social.Media.Infrastructure.Storage.Factories;
using Social.Media.Infrastructure.Storage.Options;
using Social.Media.Infrastructure.Storage.UrlServices;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;

namespace Social.Media.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConfiguration(
            IConfiguration configuration)
        {
            services.AddEndpoints(
                Assembly.GetExecutingAssembly());
            
            services.AddAuth(configuration);
            
            services.Configure<PublicStorageOptions>(
                configuration.GetSection(
                    nameof(PublicStorageOptions)));
            
            services.Configure<ApiOptions>(
                configuration.GetSection(
                    nameof(ApiOptions)));

            services.AddSingleton<S3ClientFactory>();
            
            services.AddSingleton<IPublicStorage>(sp =>
            {
                var factory = sp.GetRequiredService<S3ClientFactory>();
                var options = sp.GetRequiredService<IOptions<PublicStorageOptions>>()
                    .Value;
                
                return new PublicStorage(
                    factory.Create(
                        options.ServiceUrl,
                        options.AccessKey,
                        options.SecretKey),
                    Options.Create(options));
            });
            
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IMediaUrlService, UrlService>();

            return services;
        }
    }
}