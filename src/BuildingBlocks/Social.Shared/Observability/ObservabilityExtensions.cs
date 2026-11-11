using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Social.Shared.Observability;

public static class ObservabilityExtensions
{
    private const string DefaultOtlpEndpoint = "http://localhost:4317";
    
    private const string WolverineSource = "Wolverine";
    
    private const string YarpSource = "Yarp.ReverseProxy";

    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddObservability()
        {
            var configuration = builder.Configuration;

            var serviceName = configuration["Observability:ServiceName"]
                              ?? builder.Environment.ApplicationName;

            var environment = builder.Environment.EnvironmentName;

            var endpoint = configuration["Observability:OtlpEndpoint"]
                           ?? DefaultOtlpEndpoint;

            var otlpEndpoint = new Uri(endpoint);

            var resourceAttributes = new Dictionary<string, object>
            {
                ["deployment.environment"] = environment
            };
            
            builder.Logging.ClearProviders();

            builder.Services.AddSerilog((_, logger) =>
            {
                if (configuration.GetSection("Serilog").Exists())
                {
                    logger.ReadFrom.Configuration(configuration);
                }
                else
                {
                    logger.WriteTo.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                        "[{service.name}] {SourceContext}: " +
                        "{Message:lj}{NewLine}{Exception}");
                }

                logger
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("service.name", serviceName)
                    .Enrich.WithProperty(
                        "deployment.environment", environment)
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Protocol = OtlpProtocol.Grpc;
                        options.ResourceAttributes =
                            new Dictionary<string, object>
                            {
                                ["service.name"] = serviceName,
                                ["deployment.environment"] = environment
                            };
                    });
            });

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(
                        serviceName: serviceName,
                        serviceInstanceId: Environment.MachineName)
                    .AddAttributes(resourceAttributes))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(WolverineSource)
                    .AddOtlpExporter(options =>
                        options.Endpoint = otlpEndpoint))
                .WithTracing(tracing => tracing
                    .AddSource(WolverineSource)
                    .AddSource(YarpSource)
                    .AddAspNetCoreInstrumentation(options =>
                        options.Filter = context =>
                            !context.Request.Path
                                .StartsWithSegments("/docs")
                            && !context.Request.Path
                                .StartsWithSegments("/openapi"))
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                        options.Endpoint = otlpEndpoint));

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication UseObservability()
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} " +
                    "-> {StatusCode} in {Elapsed:0.0} ms";
            });

            return app;
        }
    }
}
