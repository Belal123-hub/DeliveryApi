using DAL.Data;
namespace DAL
{
    public class Basket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
    }

}

