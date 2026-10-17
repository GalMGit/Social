using JasperFx.CodeGeneration.Model;
using Social.Media.DI;
using Social.Shared.Endpoint;
using Social.Shared.OpenApi;
using Wolverine;
using Wolverine.Persistence.Durability;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(opt =>
{
    opt.UseRabbitMq(
            builder.Configuration.GetConnectionString(
                "RabbitMq")!)
        .AutoProvision();
    
    opt.ConfigureRabbitMq()
        .DeclareExchange("posts-events", exchange =>
        {
            exchange.ExchangeType = ExchangeType.Fanout;
            exchange.BindQueue("social-media");
        });
    
    opt.Policies.DisableConventionalLocalRouting();
    
    opt.ListenToRabbitQueue("social-media")
        .ConfigureQueue(q =>
        {
            q.IsDurable = true;
            q.AutoDelete = false;
            q.IsExclusive = false;
        });
    
    opt.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    opt.AddMediaMessaging(builder.Configuration);
});

builder.Services.AddOpenApiWithBearer();

builder.Services.AddConfiguration(builder.Configuration);

var app = builder.Build();

var api = app.MapGroup("/api/v1");
app.MapEndpoints(api);

app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();
