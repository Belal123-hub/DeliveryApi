using DTO.Enums;
namespace DTO
{
public class OrderDto
    {
        public string Id { get; set; }
        public string DeliveryTime { get; set; }
        public string OrderTime { get; set; }
        public OrderStatus Status { get; set; }
        public double Price { get; set; } 
        public List<DishBasketDto> Dishes { get; set; }
        public string Address { get; set; }
    }
}
