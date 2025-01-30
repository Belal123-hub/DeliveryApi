using DTO.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Data
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
        public string? Address { get; set; }
        public Gender Gender { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
