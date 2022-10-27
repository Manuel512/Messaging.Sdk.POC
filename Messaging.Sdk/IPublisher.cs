using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Sdk
{
    public interface IPublisher
    {
        Task Publish<T>(T message) where T : class;
    }

    public class ServiceBusPublisher : IPublisher
    {
        private readonly IBus _bus;

        public ServiceBusPublisher(IBus bus)
        {
            _bus = bus;
        }

        public async Task Publish<T>(T message) where T : class
        {
            await _bus.Publish(message);
        }
    }
}
