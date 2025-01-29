using DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Basket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } // Navigation property to User
        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
    }

}

