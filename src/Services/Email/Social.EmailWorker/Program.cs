using System.Reflection;
using Social.EmailWorker.DI;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfiguration(builder.Configuration);

builder.UseWolverine(opt =>
{
    opt.UseRabbitMq(
            builder.Configuration.GetConnectionString("RabbitMq")!)
        .AutoProvision();

    opt.ListenToRabbitQueue("social-email")
        .ConfigureQueue(q =>
        {
            q.IsDurable = true;
            q.AutoDelete = false;
            q.IsExclusive = false;
        });

    opt.Discovery.IncludeAssembly(
        Assembly.GetExecutingAssembly());
});

var host = builder.Build();

await host.RunAsync();