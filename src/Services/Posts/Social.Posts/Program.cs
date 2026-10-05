using JasperFx.CodeGeneration.Model;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Social.Posts.DI;
using Social.Shared.Endpoint;
using Social.Shared.OpenApi;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(opt =>
{
    opt.UseRabbitMq(
            builder.Configuration.GetConnectionString(
                "RabbitMq")!)
        .AutoProvision();
    
    opt.ConfigureRabbitMq()
        .DeclareExchange("users-events", exchange =>
        {
            exchange.ExchangeType = ExchangeType.Fanout;
            exchange.BindQueue("social-posts");
        });

    opt.Policies.DisableConventionalLocalRouting();

    opt.UseEntityFrameworkCoreTransactions();
    opt.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opt.Policies.UseDurableInboxOnAllListeners();

    opt.PersistMessagesWithPostgresql(
        builder.Configuration.GetConnectionString(
            "PostsDb")!,
        role: MessageStoreRole.Main);
    
    opt.ListenToRabbitQueue("social-posts")
        .ConfigureQueue(q =>
        {
            q.IsDurable = true;
            q.AutoDelete = false;
            q.IsExclusive = false;
        });
    
    opt.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    opt.AddPostsMessaging(builder.Configuration);
});

builder.Services.AddOpenApiWithBearer();

builder.Services.AddPosts(builder.Configuration);

var app = builder.Build();

await app.Services.InitializePostsAsync();

var api = app.MapGroup("/api/v1");
app.MapEndpoints(api);

app.MapOpenApi();

app.MapScalarApiReference("/docs", options =>
{
    options.WithTitle("Social API")
        .AddPreferredSecuritySchemes("Bearer")
        .AddHttpAuthentication("Bearer", auth =>
        {
            auth.Token = string.Empty;
            auth.Description = "Bearer Token";
        });
});

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();