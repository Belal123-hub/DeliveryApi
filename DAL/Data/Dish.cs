using DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class Dish
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public Decimal Price { get; set; }

        public string Description { get; set; }

        public bool IsVegetarian { get; set; }

        public string? Image { get; set; }

        public double Rating { get; set; }

        public DishCategory Category { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
    }
}
