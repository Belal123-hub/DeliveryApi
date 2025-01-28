using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class DishRating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DishId { get; set; }
        public double Rating { get; set; }
        public DateTime RatedAt { get; set; }
    }
}
