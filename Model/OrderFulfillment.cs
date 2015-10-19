using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public interface IOrderFulfillment
    {
        string ProductName { get; set; }
        double Price { get; set; }
        DateTime FulfillmentDateTime { get; set; }
    }

    public class OrderFulfillment : IOrderFulfillment
    {
        public string ProductName { get; set; }

        public double Price { get; set; }

        public DateTime FulfillmentDateTime { get; set; }
    }
}
