using System.Threading.RateLimiting;
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
        "yarp.communities.json",
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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("auth-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("posts-create-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("posts-list-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("post-by-id-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    
    options.AddPolicy("communities-create-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
            ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("communities-list-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
            ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("community-by-id-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
            ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    
    options.AddPolicy("media-create-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    
    options.AddPolicy("comment-create-ip-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() 
            ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

app.UseRouting();

app.UseCors("FrontendClient");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

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
            "/openapi/comments/v1.json")
        .AddDocument("communities", "Communities API",
            "/openapi/communities/v1.json");
});

app.MapReverseProxy();

await app.RunAsync();