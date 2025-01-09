using DAL;
using DAL.Data;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
      public interface IBasketService
    {
        Task<List<DishBasketDto>?> GetAllDishesInBasketAsync();
        Task<bool> AddDishToBasketByIdAsync(int dishId);

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

        public async Task<bool> AddDishToBasketByIdAsync(int dishId)
        {
            // Get the dish from the database
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

            if (dish == null)
                return false; // Dish not found

            // Get the user's basket (You may need to get the userId from the context or other mechanism)
            var userId = Guid.NewGuid(); // Example, replace with actual user logic
            var basketItem = await _context.Baskets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.DishId == dishId);

            if (basketItem != null)
            {
                // Dish exists in the basket, increase the amount
                basketItem.Amount += 1;
                _context.Baskets.Update(basketItem);
            }
            else
            {
                // Dish doesn't exist in the basket, add it with an amount of 1
                var newBasketItem = new Basket
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DishId = dishId,
                    Name = dish.Name,
                    Price = dish.Price,
                    Amount = 1,
                    CreateDateTime = DateTime.UtcNow,
                    ModifyDateTime = DateTime.UtcNow
                };

                await _context.Baskets.AddAsync(newBasketItem);
            }

            await _context.SaveChangesAsync();
            return true; // Success
        }
    }
}

