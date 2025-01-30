using DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class UserEditModelDto
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
