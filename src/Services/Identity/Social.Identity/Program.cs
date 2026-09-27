using JasperFx.CodeGeneration.Model;
using Scalar.AspNetCore;
using Social.Identity.DI;
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
            "RabbitMq")!);
    
    opt.Policies.DisableConventionalLocalRouting();
    
    opt.UseEntityFrameworkCoreTransactions();
    opt.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opt.Policies.UseDurableInboxOnAllListeners();
    
    opt.PersistMessagesWithPostgresql(
        builder.Configuration.GetConnectionString(
            "WolverineDb")!,
        role: MessageStoreRole.Main);
    
    opt.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    opt.AddIdentityMessaging(builder.Configuration);
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});
builder.Services.AddIdentity(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeIdentityAsync();

var api = app.MapGroup("/api/v1");
app.MapEndpoints(api);

app.MapOpenApi();
app.MapScalarApiReference("/docs",options =>
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
