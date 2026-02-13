using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class DeliveryMethod
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
