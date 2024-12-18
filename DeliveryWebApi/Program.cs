using System.Security.Claims;
using System.Text;
using Backend2024ExampleApp.Configuration;
using BLL.Configuration;
using BLL.Services;
using DAL.Configurations;
using DAL.Data;


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

            var app = builder.Build();

            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
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
