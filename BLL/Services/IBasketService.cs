using DAL.Data;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
      public interface IBasketService
    {
        Task<List<DishBasketDto>?> GetAllDishesInBasketAsync();
    }

    public class BasketService : IBasketService
    {
        private readonly ApplicationDbContext _context;

        public BasketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DishBasketDto>?> GetAllDishesInBasketAsync()
        {
            // Fetch all items in the basket
            var items = await _context.Baskets
                .ToListAsync();

            if (!items.Any())
                return null;

            // Map Basket items to DishBasketDto
            var dishBasketDtos = items.Select(item => new DishBasketDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                TotalPrice = item.Price * item.Amount, // Calculate subtotal for each item
                Amount = item.Amount,
                Image = item.Image
            }).ToList();

            return dishBasketDtos;
        }
    }
}

