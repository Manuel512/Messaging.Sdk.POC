using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Sdk
{
    public interface IConsumerConfigurator
    {
        HashSet<Type> ConsumerTypes { get; }

        void Add<TConsumer>() where TConsumer : IConsumer;
    }

    internal class ConsumerConfigurator : IConsumerConfigurator
    {
        public ConsumerConfigurator()
        {
            ConsumerTypes = new HashSet<Type>();
        }

        public HashSet<Type> ConsumerTypes { get; private set; }

        public void Add<TConsumer>() where TConsumer : IConsumer
        {
            ConsumerTypes.Add(typeof(TConsumer));
        }
    }
}
