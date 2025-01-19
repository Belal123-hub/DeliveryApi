using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public DateTime DeliveryTime { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
