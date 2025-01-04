using DTO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTO
{
    public class UserEditModelDto
    {
        public string Name { get; set; } // Required
        public DateOnly BirthDate { get; set; } // Optional, nullable
        //public Gender Gender { get; set; } // Required
        //public string Address { get; set; } // Optional, nullable
        public string PhoneNumber { get; set; } // Optional, nullable
    }
}
