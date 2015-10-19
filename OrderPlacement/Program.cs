using Core.Model;
using ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlacement
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                var originalColor = Console.ForegroundColor;
                IProductOrder order = new ProductOrder();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Product Name: ");
                order.ProductName = Console.ReadLine();

                Console.Write("Price ($): ");
                order.Price = double.Parse(Console.ReadLine());

                using(var bus = ServiceBus.ServiceBusFactory.CreateBasic(0, 1))
                {
                    var message = new QueueMessage<IProductOrder>(true);

                    order.TimeStamp = DateTime.Now;
                    message.Body = order;
                    message.Settings.IsDurable = true;
                    message.Settings.QueueName = "Product.Orders";

                    bus.Enqueue(message);
                }

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Your order of {0} - {1} has been placed successfully at {2}.", order.ProductName, order.Price.ToString("C"), order.TimeStamp);

                Console.ForegroundColor = originalColor;
            }
        }
    }
}
