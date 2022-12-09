using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Messaging.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, Action<IMessagingConfigurator> messagingConfigurator, params Assembly[] consumerAssemblies)
        {
            var configurator = new MessagingConfigurator();
            messagingConfigurator(configurator);            

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                //var assembly = Assembly.GetEntryAssembly();

                ////var consumerTypes = consumerAssemblies.SelectMany(assembly =>
                ////    assembly.GetTypes()
                ////    .Where(t => t.IsAssignableFrom(typeof(IConsumer)) && !t.IsAbstract && !t.IsInterface));

                ////x.AddConsumers(assembly);

                ////foreach (var consumer in consumerTypes)
                ////{
                ////    x.AddConsumer(consumer);
                ////}

                //x.UsingAzureServiceBus((context, cfg) =>
                //{
                //    cfg.Host(configurator.ConnectionString);

                //    ////Topics configuration
                //    //foreach (var topicConfigurator in configurator.Topics.Select(x => x.Key))
                //    //{
                //    //    topicConfigurator(cfg);
                //    //}

                //    ////Consumers configuration
                //    //foreach (var consumerTypesConfigurator in configurator.Topics.SelectMany(x => x.Value))
                //    //{
                //    //    consumerTypesConfigurator(context, cfg);
                //    //}

                //    cfg.ConfigureEndpoints(context);
                //});

                var assembly = Assembly.GetEntryAssembly();
                var interfaceType = typeof(IConsumer);
                var consumers = assembly?.GetTypes()
                    .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                    .ToList() ?? new List<Type>();

                consumers.ForEach(c => x.AddConsumer(c));

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configurator.ConnectionString);

                    foreach (var consumer in consumers)
                    {
                        var messageType = consumer.GetInterfaces().Where(i => i.IsGenericType).FirstOrDefault()?.GetGenericArguments().FirstOrDefault();

                        if (messageType != null)
                        {
                            string topic = $"{messageType.Namespace?.ToLower()}/{messageType.Name.ToLower()}";
                            string subscription = KebabCaseEndpointNameFormatter.Instance.SanitizeName(consumer.Name.Replace("Consumer", string.Empty));

                            cfg.SubscriptionEndpoint(subscription, topic, ec =>
                            {
                                ec.ConfigureConsumer(context, consumer);
                                ec.PublishFaults = false;
                            });
                        }
                    }

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
