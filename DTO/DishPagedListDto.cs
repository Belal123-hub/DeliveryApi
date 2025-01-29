using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DishPagedListDto
    {
        public List<DishDto>? Dishes { get; set; }
        public PageInfoModel Paginatin { get; set; }
    }

    public class PageInfoModel 
    {
        public int Size { get; set; } // Number of items per page
        public int Count { get; set; } // Total number of items
        public int Current { get; set; } // Current page number
    }
}
