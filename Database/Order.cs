using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        [ForeignKey(nameof(PaymentMethod))]
        public int PaymentMethodId { get; set; }
        [ForeignKey(nameof(DeliveryMethod))]
        public int DeliveryMethodId { get; set; }
        [ForeignKey(nameof(Address))]
        public int ShippingAddressId { get; set; }
        [ForeignKey(nameof(Address))]
        public int BillingAddressId { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal Cost { get; set; }
        public bool Finished { get; set; }


        public Customer Customer { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
