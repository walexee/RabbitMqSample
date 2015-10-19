using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServiceBus
{
    public enum ExchangeTypeOption
    {
        None,
        Direct,
        FanOut,
        Topic,
        Headers
    }
}
