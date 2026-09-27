using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Social.Shared.Endpoint;

public static class EndpointExtensions
{
    extension(IServiceCollection services)
    {
        public void AddEndpoints(Assembly assembly)
        {
            ServiceDescriptor[] serviceDescriptors = [
                .. assembly
                    .DefinedTypes
                    .Where(t => t is {IsAbstract: false, IsInterface: false }
                                && t.IsAssignableTo(typeof(IEndpoint)))
                    .Select(t => ServiceDescriptor.Transient(typeof(IEndpoint), t))
            ];

            services.TryAddEnumerable(serviceDescriptors);
        }
    }

    extension(WebApplication app)
    {
        public IApplicationBuilder MapEndpoints(RouteGroupBuilder? routeGroupBuilder = null)
        {
            var endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder = routeGroupBuilder is null
                ? app
                : routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
                endpoint.MapEndpoint(builder);

            return app;
        }
    }
}