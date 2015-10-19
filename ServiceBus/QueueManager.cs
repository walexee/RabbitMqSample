using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class QueueManager : IQueueManager
    {
        private readonly IModel _model;

        public QueueManager(IModel model)
        {
            _model = model;
        }

        public void CreateQueue(QueueSettings settings)
        {
            _model.QueueDeclare(settings.QueueName, settings.IsDurable, settings.IsExclusive, settings.AutoDelete, null);
        }

        public string CreateQueue()
        {
            var result = _model.QueueDeclare();

            return result.QueueName;
        }

        public void CreateExchange(ExchangeSettings settings)
        {
            _model.ExchangeDeclare(settings.ExchangeName, settings.ExchangeType, settings.IsDurable, settings.AutoDelete, null);
        }

        public void BindQueue(string queueName, string exchangeName, string routeKey)
        {
            _model.ExchangeBind(queueName, exchangeName, routeKey);
        }

        public bool QueueExists(string queueName)
        {
            var exists = false;
            try
            {
                _model.QueueDeclarePassive(queueName);
                exists = true;
            }
            catch
            {
                
            }

            return exists;
        }

        public bool ExchangeExists(string exchangeName)
        {
            var exists = false;
            try
            {
                _model.ExchangeDeclarePassive(exchangeName);
                exists = true;
            }
            catch
            {

            }

            return exists;
        }
    }
}
