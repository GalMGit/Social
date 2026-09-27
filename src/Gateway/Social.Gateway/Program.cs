using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot(
        "ocelot-configuration", 
        builder.Environment);

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.UseOcelot();
await app.RunAsync();
