using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Models
{
    public class Order
    {
        [Key]
        public Guid ID { get; set; }
    }
}
