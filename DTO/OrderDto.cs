using DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
public class OrderDto
    {
        public string Id { get; set; } // Unique identifier for the order (UUID as string)
        public string DeliveryTime { get; set; } // Scheduled delivery time (ISO 8601 format)
        public string OrderTime { get; set; } // Time when the order was placed (ISO 8601 format)
        public OrderStatus Status { get; set; } // Current status of the order (e.g., "Pending", "Delivered")
        public double Price { get; set; } // Total price of the order
        public List<DishBasketDto> Dishes { get; set; } // List of dishes in the order
        public string Address { get; set; } // Delivery address
    }
}
