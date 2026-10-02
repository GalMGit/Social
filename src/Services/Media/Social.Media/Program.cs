using Social.Media.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddConfiguration(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();

await app.RunAsync();
