using Microsoft.EntityFrameworkCore;
using DeliveryWebApi.Data.Models;
namespace DeliveryWebApi.Data
{
    public class ApiDbContext: DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext>options):base(options)
        {

        }
        public DbSet<User>Users { get; set; }
    }
}
