﻿using MassTransit;
using Messaging.Contracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Sdk.Client
{
    public class PublisherHostedService : BackgroundService
    {
        private readonly IBus _bus;

        public PublisherHostedService(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var member = new UserEvent
            {
                Id = 1,
                Email = "manuel@test.com",
                FirstName = "Manuel",
                LastName = "Pérez",
                UserName = "manu"
            };

            await Task.Delay(10000);

            await _bus.Publish(member, stoppingToken);
        }
    }
}
