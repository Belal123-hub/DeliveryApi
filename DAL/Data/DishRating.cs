using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class DishRating
    {
        public Guid UserId { get; set; }
        public Guid DishId { get; set; }
        public int RatingScore { get; set; } // Rating score (e.g., 1 to 5)

        // Navigation properties
        public User User { get; set; }
        public Dish Dish { get; set; }
    }
}
