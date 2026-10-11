using Scalar.AspNetCore;
using Social.Shared.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile(
        "yarp.auth.json",
        optional: false,
        reloadOnChange: true)
    .AddJsonFile(
        "yarp.posts.json",
        optional: false,
        reloadOnChange: true)
    .AddJsonFile(
        "yarp.users.json",
        optional: false,
        reloadOnChange: true)
    .AddJsonFile(
        "yarp.comments.json",
        optional: false,
        reloadOnChange: true)
    .AddJsonFile(
        "yarp.media.json",
        optional: false,
        reloadOnChange: true)
    .AddJsonFile(
        "yarp.openapi.json",
        optional: false,
        reloadOnChange: true);

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy =>
    {
        policy
            .AddAuthenticationSchemes("Bearer")
            .RequireAuthenticatedUser();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();

app.UseCors("FrontendClient");

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
        .AddDocument("identity", "Identity API",
            "/openapi/identity/v1.json")
        .AddDocument("posts", "Posts API",
            "/openapi/posts/v1.json")
        .AddDocument("media", "Media API",
            "/openapi/media/v1.json")
        .AddDocument("users", "Users API",
            "/openapi/users/v1.json")
        .AddDocument("comments", "Comments API",
            "/openapi/comments/v1.json");
});

app.MapReverseProxy();

await app.RunAsync();