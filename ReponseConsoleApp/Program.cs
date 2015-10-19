
using Core.Model;
using ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReponseConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "Product.Orders.Request";
            var bus = ServiceBusFactory.CreateBasic(0, 1);

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Func<ProductOrder, OrderFulfillment> receiveHander = (result) =>
            {
                Console.WriteLine("Your order of {0} - {1} was received at {2}.",
                    result.ProductName,
                    result.Price.ToString("C"),
                    DateTime.Now.ToLongTimeString());

                var fulfillment = new OrderFulfillment
                {
                    FulfillmentDateTime = DateTime.Now,
                    Price = result.Price,
                    ProductName = result.ProductName
                };

                return fulfillment;

            };

            bus.HandleRequest(queueName, receiveHander);
        }
    }
}
