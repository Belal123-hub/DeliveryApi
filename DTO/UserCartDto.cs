namespace DTO
{
    public class UserCartDto
    {
        public List<DishBasketDto> Items { get; set; }
        public double TotalPrice { get; set; }
    }
}
