using System;

namespace ServiceBus
{
    public interface IBusControl : IDisposable
    {
        void Publish<T>(ExchangeMessage<T> message);
        void Enqueue<T>(QueueMessage<T> message);
        QueueMessageResult<T> Dequeue<T>(string queueName);
        QueueMessageResult<string> Dequeue(string queueName);
        S Request<T, S>(QueueMessage<T> message, TimeSpan timeout);
        void HandleRequest<T, S>(string queueName, Func<T, S> requestHandler);
        void HandleReceive<T>(string queueName, Action<T> receiveEventHandler);
        IQueueManager QueueManager { get; }
    }
}
