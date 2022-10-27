using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace Messaging.Sdk
{
    internal class ServiceBusKebabCaseEntityNameFormatter<T> : KebabCaseEndpointNameFormatter, IMessageEntityNameFormatter<T> where T : class
    {
        private readonly string _messageTypeName;

        public ServiceBusKebabCaseEntityNameFormatter()
        {
            var messageType = typeof(T);
            string nameSpace = messageType.Namespace.ToLower();
            string topic = SanitizeName(messageType.Name) + 's';

            _messageTypeName = nameSpace + '/' + topic;
        }

        public string FormatEntityName()
        {
            return _messageTypeName;
        }
    }
}
