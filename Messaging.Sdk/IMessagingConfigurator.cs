using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using MassTransit.AzureServiceBusTransport.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace Messaging.Sdk
{
    public interface IMessagingConfigurator
    {
        void AddConnectionString(string connectionString);
        void AddTopic<TMessage>() where TMessage : class;
        void AddTopic<TMessage>(Action<IConsumerConfigurator> configurator) where TMessage : class;
    }

    internal class MessagingConfigurator : IMessagingConfigurator
    {
        public MessagingConfigurator()
        {
            Topics = new Dictionary<Action<IServiceBusBusFactoryConfigurator>, ICollection<Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator>>>();
            Consumers = new HashSet<Type>();
        }

        public string ConnectionString { get; private set; } = default!;
        public Dictionary<Action<IServiceBusBusFactoryConfigurator>, ICollection<Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator>>> Topics { get; private set; }
        public HashSet<Type> Consumers { get; private set; }

        public void AddConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void AddTopic<TMessage>() where TMessage : class
        {
            Topics.Add(RegisterMessageTopology<TMessage>(), new List<Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator>>());
        }

        public void AddTopic<TMessage>(Action<IConsumerConfigurator> configurator) where TMessage : class
        {
            var consumerConfig = new ConsumerConfigurator();
            configurator(consumerConfig);

            var messageType = typeof(TMessage);

            var consumerConfigs = new List<Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator>>();

            foreach (var consumertype in consumerConfig.ConsumerTypes)
            {
                var consumerInterface = consumertype.GetInterfaces().Where(x => x.GenericTypeArguments.Length > 0).FirstOrDefault();
                var consumerMessageType = consumerInterface?.GetGenericArguments().FirstOrDefault();
                if (consumerMessageType != messageType)
                {
                    throw new InvalidOperationException($"Consumer: {consumertype.Name} message type does not match Topic message type: {messageType.Name}");
                }

                Consumers.Add(consumertype);
                consumerConfigs.Add((context, cfg) =>
                {
                    //string subscription = consumertype.Name.ToLower();
                    string subscription = new KebabCaseEndpointNameFormatter(false).SanitizeName(consumertype.Name);
                    cfg.SubscriptionEndpoint<TMessage>(subscription, e =>
                    {
                        e.PublishFaults = false;
                        e.ConfigureConsumer(context, consumertype);
                    });
                });
            }

            Topics.Add(RegisterMessageTopology<TMessage>(), consumerConfigs);
        }

        private Action<IServiceBusBusFactoryConfigurator> RegisterMessageTopology<TMessage>() where TMessage : class
        {
            return (cfg) =>
            {
                //Custom topic naming
                cfg.Message<TMessage>(topology =>
                {
                    topology.SetEntityNameFormatter(new ServiceBusKebabCaseEntityNameFormatter<TMessage>());
                });
            };
        }
    }
}
