using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public interface IProductOrder
    {
        string ProductName { get; set; }
        double Price { get; set; }
        DateTime TimeStamp { get; set; }
    }

    public class ProductOrder : IProductOrder
    {

        public string ProductName { get; set; }

        public double Price { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
