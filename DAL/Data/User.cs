using DTO.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Data
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public string FullName { get; set; } // Changed from Name to FullName
        public DateTime? BirthDate { get; set; } // Changed from DateOnly to DateTime and made nullable
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
        public string? Address { get; set; } // Added Address
        public Gender Gender { get; set; } // Added Gender
        public string? PhoneNumber { get; set; } // Added PhoneNumber
    }
}
