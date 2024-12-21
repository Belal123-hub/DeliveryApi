using System.Security.Claims;
using System.Text;
using Backend2024ExampleApp.Configuration;
using BLL.Configuration;
using BLL.Services;
using DAL.Configurations;
using DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Backend2024ExampleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.ConfigureDal();
            builder.ConfigureBll();

            // Configure JWT
            var jwtSection = builder.Configuration.GetSection("JwtBearerTokenSettings");
            builder.Services.Configure<JwtBearerTokenSettings>(jwtSection);

            var jwtConfiguration = jwtSection.Get<JwtBearerTokenSettings>();
            var key = Encoding.ASCII.GetBytes(jwtConfiguration.secretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = jwtConfiguration.Audience,
                    ValidIssuer = jwtConfiguration.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = true,
                    LifetimeValidator = (before, expires, token, parameters) =>
                    {
                        var utcNow = DateTime.UtcNow;
                        return before <= utcNow && utcNow < expires;
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationRoleNames.Administrator,
                    new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole(ApplicationRoleNames.Administrator)
                    .RequireClaim(ClaimTypes.Role, ApplicationRoleNames.Administrator)
                    .Build());
            });

            var app = builder.Build();

            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();
            await app.ConfigureIdentityAsync();
            app.Run();
        }
    }
}
