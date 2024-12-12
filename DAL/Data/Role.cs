using Microsoft.AspNetCore.Identity;

namespace DAL.Data;

public class Role : IdentityRole<Guid>, IBaseEntity
{
    public DateTime CreateDateTime { get; set; }
    public DateTime ModifyDateTime { get; set; }
    public DateTime? DeleteDateTime { get; set; }
}