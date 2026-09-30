using Microsoft.OpenApi;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Scalar.AspNetCore;
using Social.Shared.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth(builder.Configuration);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot(
        "ocelot-configuration", 
        builder.Environment);

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapScalarApiReference("/docs", options =>
{
    options
        .WithTitle("Social API")
        .AddPreferredSecuritySchemes("Bearer")
        .AddHttpAuthentication("Bearer", auth =>
        {
            auth.Token = string.Empty;
            auth.Description = "Bearer Token";
        })
        .AddDocument(
            "identity",
            "Identity API",
            "/openapi/identity/v1.json",
            isDefault: true);
});

app.UseWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/docs"),
    branch => branch.UseOcelot());

await app.RunAsync();
