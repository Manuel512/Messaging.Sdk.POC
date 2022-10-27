using MassTransit;
using Messaging.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Sdk.Client
{
    internal class UserEventCreatedConsumer : IConsumer<UserEvent>
    {
        public Task Consume(ConsumeContext<UserEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}
