using System.Reflection;
using Social.Media.Application.Abstractions.IServices.IMediaServices;
using Social.Media.Infrastructure.Storage;
using Social.Media.Infrastructure.Storage.Factories;
using Social.Media.Infrastructure.Storage.Options;
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
            
            services.AddScoped<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}