using Microsoft.AspNetCore.Identity;

namespace DeliveryWebApi.Data.Models
{
    public class User:IdentityUser<Guid>,IBaseEntity
    {
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
        public string? SocialNumber { get; set; }
    }
}
