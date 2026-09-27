using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot(
        "ocelot-configuration", 
        builder.Environment);

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.MapScalarApiReference("/docs", options =>
{
    options
        .WithTitle("Social API")
        .AddDocument(
            "identity",
            "Identity API",
            "/openapi/identity/v1.json",
            isDefault: true);
});

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/docs"),
    branch =>
    {
        branch.UseOcelot();
    });

await app.RunAsync();
