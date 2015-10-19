using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace ServiceBus
{
    public class BasicBusControl : IBusControl
    {
        private readonly IConnection _connection;
        private readonly IModel _model;
        private readonly IQueueManager _queueManager;
        private string _responseQueue;

        internal BasicBusControl(IConnection connection)
        {
            _connection = connection;
            _model = connection.CreateModel();
            _queueManager = new QueueManager(_model);
        }

        private string ResponseQueue
        {
            get
            {
                if (string.IsNullOrEmpty(_responseQueue))
                {
                    _responseQueue = QueueManager.CreateQueue();
                }

                return _responseQueue;
            }
        }

        internal IModel Model
        {
            get { return _model; } 
        }

        public IQueueManager QueueManager
        {
            get { return _queueManager; }
        }

        public void Publish<T>(ExchangeMessage<T> message)
        {
            var settings = message.Settings;

            if(string.IsNullOrEmpty(settings.ExchangeName))
            {
                throw new ArgumentNullException("ExchangeName cannot be null");
            }

            var properties = _model.CreateBasicProperties();
            var body = GetMessageBytes(message.Body);
            

            properties.Persistent = settings.IsDurable;

            if (message.CreateExchangeIfNotExist && !_queueManager.ExchangeExists(settings.ExchangeName))
            {
                QueueManager.CreateExchange(settings);
            }

            _model.BasicPublish(settings.ExchangeName, string.Empty, properties, body);
        }

        public void Enqueue<T>(QueueMessage<T> message)
        {
            if(string.IsNullOrEmpty(message.Settings.QueueName))
            {
                throw new ArgumentNullException("Queue name cannot be null");
            }

            var properties = _model.CreateBasicProperties();
            var body = GetMessageBytes(message.Body);
            var settings = message.Settings;

            properties.Persistent = settings.IsDurable;
            properties.CorrelationId = message.CorrelationId;

            if(message.CreateQueueIfNotExist && !QueueManager.QueueExists(settings.QueueName))
            {
                QueueManager.CreateQueue(settings);
            }

            _model.BasicPublish(string.Empty, settings.QueueName, properties, body);
        }

        public QueueMessageResult<T> Dequeue<T>(string queueName)
        {
            var messageResult = Dequeue(queueName);
            var result = new QueueMessageResult<T>();

            if(messageResult.Success)
            {
                result.Body = JsonConvert.DeserializeObject<T>(messageResult.Body);
                result.Success = true;
                result.ErrorMessage = messageResult.ErrorMessage;
                result.CorrelationId = messageResult.CorrelationId;
            }

            return result;
        }

        public QueueMessageResult<string> Dequeue(string queueName)
        {
            var result = new QueueMessageResult<string>();
            var consumer = new QueueingBasicConsumer(_model);
            
            try
            {
                _model.BasicConsume(queueName, false, consumer);

                var deliverEventArgs = consumer.Queue.Dequeue();

                if (deliverEventArgs != null && deliverEventArgs.Body != null)
                {
                    result.Body = Encoding.Default.GetString(deliverEventArgs.Body);
                    result.Success = true;
                    result.CorrelationId = deliverEventArgs.BasicProperties.CorrelationId;

                    _model.BasicAck(deliverEventArgs.DeliveryTag, false);
                }
            }
            catch (Exception ex) 
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public void HandleReceive<T>(string queueName, Action<T> receiveEventHandler)
        {
            _model.QueueDeclarePassive(queueName);

            var evtBasicConsumer = new EventingBasicConsumer(_model);

            evtBasicConsumer.Received += (x, y) => {
                var body = DeserializeMessage<T>(y.Body);

                receiveEventHandler(body);
                _model.BasicAck(y.DeliveryTag, false);
            };

            _model.BasicConsume(queueName, false, evtBasicConsumer);
        }

        public S Request<T, S>(QueueMessage<T> message, TimeSpan timeout)
        {
            var body = GetMessageBytes(message.Body);
            var properties = _model.CreateBasicProperties();
            var timeoutAt = DateTime.Now + timeout;
            S reponseMessage = default(S);

            properties.ReplyTo = ResponseQueue;
            properties.CorrelationId = message.CorrelationId;

            if (message.CreateQueueIfNotExist && !QueueManager.QueueExists(message.Settings.QueueName))
            {
                QueueManager.CreateQueue(message.Settings);
            }

            _model.BasicPublish(string.Empty, message.Settings.QueueName, properties, body);

            while(DateTime.Now <= timeoutAt)
            {
                var response = Dequeue<S>(ResponseQueue);

                if(response.Success && response.CorrelationId == properties.CorrelationId)
                {
                    reponseMessage = response.Body;
                    break;
                }
            }

            return reponseMessage;
        }

        public void HandleRequest<T,S>(string queueName, Func<T, S> requestHandler)
        {
            if (!QueueManager.QueueExists(queueName))
            {
                throw new EntryPointNotFoundException("Queue " + queueName + " does not exist.");
            }

            var evtBasicConsumer = new EventingBasicConsumer(_model);

            evtBasicConsumer.Received += (x, y) =>
            {
                var body = DeserializeMessage<T>(y.Body);
                var response = requestHandler(body);

                if(!string.IsNullOrEmpty(y.BasicProperties.ReplyTo))
                {
                    var message = new QueueMessage<S>();

                    message.Body = response;
                    message.CorrelationId = y.BasicProperties.CorrelationId;
                    message.Settings.QueueName = y.BasicProperties.ReplyTo;

                    Enqueue(message);
                }

                _model.BasicAck(y.DeliveryTag, false);
            };

            _model.BasicConsume(queueName, false, evtBasicConsumer);
        }

        private byte[] GetMessageBytes<T>(T body)
        {
            string messageString = string.Empty;

            if(body is string)
            {
                messageString = body.ToString();
            }
            else 
            {
                messageString = JsonConvert.SerializeObject(body);
            }

            var messageBuffer = Encoding.Default.GetBytes(messageString);

            return messageBuffer;
        }

        private T DeserializeMessage<T>(byte[] body)
        {
            var bodyString = Encoding.Default.GetString(body);

            return JsonConvert.DeserializeObject<T>(bodyString);
        }

        public void Dispose()
        {
            if (_model != null)
            {
                if (_model.IsOpen) _model.Abort();

                _model.Dispose();
            }

            if (_connection != null)
            {
                if (_connection.IsOpen) _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
