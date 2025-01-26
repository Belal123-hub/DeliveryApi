using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UserRegisterModelDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
