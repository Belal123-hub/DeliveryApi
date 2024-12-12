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

            var adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator);

            if(adminRole == null)
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.Administrator
                });

                if(!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create role {ApplicationRoleNames.Administrator}. Errors: {roleResult.Errors}");
                }

                adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator);
            }

            var config = app.Configuration.GetSection("AdminCredentials");

            var adminUser = await userManager.FindByEmailAsync(config["Email"]);
            if(adminUser == null)
            {
                var user = new User
                {
                    UserName = config["Email"],
                    Email = config["Email"],
                    Name = ApplicationRoleNames.Administrator,
                    BirthDate = new DateOnly(2000, 1, 1)
                };

                var userResult = await userManager.CreateAsync(user, config["Password"]);
                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create user {config["Email"]}. Errors: {userResult.Errors}");
                }

                adminUser = await userManager.FindByEmailAsync(config["Email"]);
            }

            var isInRole = await userManager.IsInRoleAsync(adminUser, adminRole.Name);
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(adminUser, adminUser.Name);
            }
        }
    }
}
