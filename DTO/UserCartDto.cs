using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UserCartDto
    {
        public List<DishBasketDto> Items { get; set; }
        public double TotalPrice { get; set; }
    }
}
