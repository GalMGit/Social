using JasperFx.CodeGeneration.Model;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Social.Posts.DI;
using Social.Shared.Endpoint;
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

    opt.Policies.DisableConventionalLocalRouting();

    opt.UseEntityFrameworkCoreTransactions();
    opt.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opt.Policies.UseDurableInboxOnAllListeners();

    opt.PersistMessagesWithPostgresql(
        builder.Configuration.GetConnectionString(
            "WolverineDb")!,
        role: MessageStoreRole.Main);

    opt.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    opt.AddPostsMessaging(builder.Configuration);
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        };
        document.Servers = [];

        document.Components ??= new OpenApiComponents();
        document.AddComponent("Bearer", bearerScheme);

        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        };

        foreach (var operation in document.Paths.Values
                     .SelectMany(path => path.Operations!))
        {
            operation.Value.Security ??= [];
            operation.Value.Security.Add(securityRequirement);
        }

        return Task.CompletedTask;
    });
});

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