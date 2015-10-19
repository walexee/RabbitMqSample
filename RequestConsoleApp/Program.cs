using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using ServiceBus;

namespace RequestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = ServiceBus.ServiceBusFactory.CreateBasic(0, 1);
            var settings = new QueueSettings();

            settings.QueueName = "Product.Orders.Request";
            settings.IsDurable = true;
            //settings.IsExclusive = true;

            //if(!bus.QueueManager.QueueExists(settings.QueueName))
            //{
            //    bus.QueueManager.CreateQueue(settings);
            //}

            while (true)
            {
                var originalColor = Console.ForegroundColor;
                var order = new ProductOrder();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Product Name: ");
                order.ProductName = Console.ReadLine();

                Console.Write("Price ($): ");
                order.Price = double.Parse(Console.ReadLine());
                order.TimeStamp = DateTime.Now;

                Task.Factory.StartNew(() =>
                {
                    var message = new QueueMessage<ProductOrder> { Body = order, Settings = settings };
                    var response = bus.Request<ProductOrder, OrderFulfillment>(message, new TimeSpan(0, 1, 0));

                    Console.WriteLine("{2}: Your order of {0} - {1} has been fulfilled.", response.ProductName, response.Price.ToString("C"), response.FulfillmentDateTime);
                });


                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Your order of {0} - {1} has been placed successfully at {2}.", order.ProductName, order.Price.ToString("C"), order.TimeStamp);
            }
        }
    }
}
