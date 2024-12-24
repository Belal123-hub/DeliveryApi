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

        public int Id { get; set; }

        public int Price { get; set; }

        public string Description { get; set; }

        public bool IsVegetarian { get; set; }

        public string Image { get; set; }

        public double Rating { get; set; }

        //public Category category { get; set; }
    }
}
