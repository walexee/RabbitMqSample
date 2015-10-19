using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class QueueMessage<T>
    {
        private readonly bool _createQueueIfNotExist;

        public QueueMessage():this(false)
        {
        }

        public QueueMessage(bool createQueueIfNotExist)
        {
            _createQueueIfNotExist = createQueueIfNotExist;
            Settings = new QueueSettings();
            CorrelationId = Guid.NewGuid().ToString();
        }

        public T Body { get; set; }
        public string CorrelationId { get; set; }
        public bool CreateQueueIfNotExist 
        {
            get { return _createQueueIfNotExist; }
        }
        public QueueSettings Settings { get; set; }
    }
}
