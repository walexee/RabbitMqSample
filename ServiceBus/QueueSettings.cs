using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class QueueSettings
    {
        public QueueSettings()
        {
            RouteKey = string.Empty;
        }

        public string QueueName { get; set; }
        public string RouteKey { get; set; }
        public bool IsExclusive { get; set; }
        public bool IsDurable { get; set; }
        public bool AutoDelete { get; set; }
    }
}
