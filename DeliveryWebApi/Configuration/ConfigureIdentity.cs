using DAL.Configurations;
using DAL.Data;
using Microsoft.AspNetCore.Identity;

namespace Backend2024ExampleApp.Configuration
{
    public static class ConfigureIdentity
    {
        public static async Task ConfigureIdentityAsync(this WebApplication app)
        {
            using var serviceScope = app.Services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
        }
    }
}