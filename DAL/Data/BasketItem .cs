namespace DAL.Data
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid BasketId { get; set; }
        public Basket Basket { get; set; }
        public Guid DishId { get; set; }
        public Dish Dish { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public string? Image { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
    }

}
