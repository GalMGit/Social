using Social.Media.DI;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;
using Social.Shared.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiWithBearer();

builder.Services.AddConfiguration(builder.Configuration);

var app = builder.Build();

var api = app.MapGroup("/api/v1");
app.MapEndpoints(api);

app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();
