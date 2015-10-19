using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class ExchangeMessage<T>
    {
        private readonly bool _createExchangeIfNotExist;

        public ExchangeMessage():this(false)
        {
        }

        public ExchangeMessage(bool exchangeQueueIfNotExist)
        {
            _createExchangeIfNotExist = exchangeQueueIfNotExist;
            Settings = new ExchangeSettings();
        }

        public T Body { get; set; }
        public bool CreateExchangeIfNotExist 
        {
            get { return _createExchangeIfNotExist; }
        }
        public ExchangeSettings Settings { get; set; }
    }
}
