using MassTransit;
using Messaging.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Sdk.Client
{
    public class UserEventCreatedConsumer : IConsumer<UserEvent>
    {
        private readonly ILogger<UserEventCreatedConsumer> _logger;

        public UserEventCreatedConsumer(ILogger<UserEventCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<UserEvent> context)
        {
            _logger.LogInformation("I log user event {fullname}", context.Message.FullName);
            throw new Exception("Some error");
            return Task.CompletedTask;
        }
    }
}
