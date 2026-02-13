using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(City))]
        public int CityId { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public int PostCode { get; set; }
        public City City { get; set; }

        public ICollection<Order> ShippingOrders { get; set; } = new List<Order>();
        public ICollection<Order> BillingOrders { get; set; } = new List<Order>();
    }
}
