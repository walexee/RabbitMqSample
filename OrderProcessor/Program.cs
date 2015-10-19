using Core.Model;
using ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "Product.Orders";
            var bus = ServiceBusFactory.CreateBasic(0, 1);

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Action<ProductOrder> receiveHander = (result) =>
            {
                result.TimeStamp = DateTime.Now;
                    Console.WriteLine("Your order of {0} - {1} has been processed at {2}.",
                        result.ProductName,
                        result.Price.ToString("C"),
                        result.TimeStamp.ToLongTimeString());
            };

            bus.HandleReceive(queueName, receiveHander);
            //bus.Dispose();
        }
    }
}
