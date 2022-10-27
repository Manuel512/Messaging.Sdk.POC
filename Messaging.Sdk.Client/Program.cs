using Microsoft.Extensions.Hosting;
using Messaging.Sdk;
using Microsoft.Extensions.Configuration;
using Messaging.Sdk.Client;
using Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMessaging(cfg =>
        {
            var config = hostContext.Configuration;
            cfg.AddConnectionString(config.GetConnectionString("ServiceBus"));

            cfg.AddTopic<UserEvent>(consumer =>
            {
                consumer.Add<UserEventCreatedConsumer>();
            });
        });

        services.AddHostedService<PublisherHostedService>();
    });
