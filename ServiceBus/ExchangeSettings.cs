using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class ExchangeSettings
    {
        public ExchangeSettings()
        {
            ExchangeName = string.Empty;
            RouteKey = string.Empty;
        }

        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string RouteKey { get; set; }
        public bool IsDurable { get; set; }
        public bool AutoDelete { get; set; }
    }
}
