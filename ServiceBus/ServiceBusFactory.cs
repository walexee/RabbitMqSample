using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public static class ServiceBusFactory
    {
        public static IBusControl CreateBasic(ushort prefetchSize, ushort prefetchCount = 1)
        {
            string hostname = "localhost";
            string username = "guest";
            string password = "guest";

            //The two below settings are just to illustrate how they can be used but we are not using them in
            //this sample as we will use the defaults
            string virtualHost = string.Empty;
            int port = 0;

            var connectionFactory = new ConnectionFactory
            {
                HostName = hostname,
                UserName = username,
                Password = password
            };

            if (!string.IsNullOrEmpty(virtualHost))
            {
                connectionFactory.VirtualHost = virtualHost;
            }

            if (port > 0)
            {
                connectionFactory.Port = port;
            }

            var connection = connectionFactory.CreateConnection();
            var bus = new BasicBusControl(connection);

            bus.Model.BasicQos(prefetchSize, prefetchCount, false);

            return bus;
        }
    }
}
