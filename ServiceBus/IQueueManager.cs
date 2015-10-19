using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public interface IQueueManager
    {
        void CreateQueue(QueueSettings settings);
        string CreateQueue();
        void CreateExchange(ExchangeSettings settings);
        void BindQueue(string queueName, string exchangeName, string routeKey);
        bool QueueExists(string queueName);
        bool ExchangeExists(string exchangeName);
    }
}
