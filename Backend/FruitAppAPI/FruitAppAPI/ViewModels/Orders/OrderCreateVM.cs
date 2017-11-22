using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.ViewModels.Orders
{
    public class OrderCreateVM
    {
        public float Quantity { get; set; }
        public string Fruit { get; set; }
        public List<string> Certificates { get; set; }
        public Guid ClientID { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
