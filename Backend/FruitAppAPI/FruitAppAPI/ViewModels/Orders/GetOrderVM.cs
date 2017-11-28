using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.ViewModels.Orders
{
    public class GetOrderVM
    {
        public string OrderId { get; set; }
        public string Fruit { get; set; }
        public double Quantity { get; set; }
        public double PendingQuantity { get; set; }
    }
}
