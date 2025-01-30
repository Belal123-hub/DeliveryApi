using DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class UserRegisterModelDto
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string Email { get; set; }

        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
