using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Messaging.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, Action<IMessagingConfigurator> messagingConfigurator)
        {
            var configurator = new MessagingConfigurator();
            messagingConfigurator(configurator);

            services.AddMassTransit(busConfigurator =>
            {
                //busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

                foreach (var consumer in configurator.Consumers)
                {
                    busConfigurator.AddConsumer(consumer);
                }

                busConfigurator.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configurator.ConnectionString);
                    
                    //Topics configuration
                    foreach (var topicConfigurator in configurator.Topics.Select(x => x.Key))
                    {
                        topicConfigurator(cfg);
                    }
                    
                    //Consumers configuration
                    foreach (var consumerTypesConfigurator in configurator.Topics.SelectMany(x => x.Value))
                    {
                        consumerTypesConfigurator(context, cfg);
                    }

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
