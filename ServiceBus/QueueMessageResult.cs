
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class QueueMessageResult<T>
    {
        public T Body { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public string CorrelationId { get; set; }
    }
}
