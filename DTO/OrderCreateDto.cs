using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class OrderCreateDto
    {
        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        [MinLength(1)]
        public string Address { get; set; } 
    }
}
