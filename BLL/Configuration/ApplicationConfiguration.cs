
using BLL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Configuration
{
    public static class ApplicationConfiguration
    {
        public static void ConfigureBll(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IDishesService, DishService>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
        }
    }
}
