using JasperFx.CodeGeneration.Model;
using Social.Shared.Endpoint;
using Social.Shared.OpenApi;
using Social.Users.DI;
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
            "UsersDb")!,
        role: MessageStoreRole.Main);

    opt.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    opt.AddUsersMessaging(builder.Configuration);
});

builder.Services.AddOpenApiWithBearer();

builder.Services.AddUsers(builder.Configuration);

var app = builder.Build();

var api = app.MapGroup("/api/v1");
app.MapEndpoints(api);

await app.Services.InitializeUsersAsync();

app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();
