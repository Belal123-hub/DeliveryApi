namespace DTO
{
    public class DishBasketDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Amount { get; set; }
        public string? Image { get; set; } 
    }
}
