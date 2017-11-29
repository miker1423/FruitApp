using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Models
{
    public class QueueModel
    {
        public string Message { get; set; }
        public ACTIONS Action { get; set; }
        public float PrevQuantity { get; set; }
        public float ChangeQuantity { get; set; }
        public float PostQuantity { get; set; }
        public Guid TransactionID { get; set; }
    }
}
