using DAL;
using DAL.Data;
using DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz.Logging;

namespace BLL.Services
{
    public interface IBasketService
    {
        Task<List<DishBasketDto>?> GetAllDishesInBasketAsync(Guid userId);
        Task<DishBasketDto?> AddDishToBasketAsync(Guid userId, Guid dishId);
        Task<bool> UpdateDishQuantityInBasketAsync(Guid dishId, bool increase);
        Task<bool> ClearBasketAsync(Guid userId);
    }

    public class BasketService : IBasketService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BasketService> _logger; // Declare the logger

        // Modify the constructor to accept ILogger<BasketService>
        public BasketService(ApplicationDbContext context, ILogger<BasketService> logger)
        {
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));  // Check for null logger
        }

        public async Task<List<DishBasketDto>?> GetAllDishesInBasketAsync(Guid userId)
        {
            // Fetch the user's active basket (where DeleteDateTime is null)
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.DeleteDateTime == null);

            if (basket == null || !basket.Items.Any())
                return null;

            // Map the basket items to DishBasketDto
            var dishBasketDtos = basket.Items.Select(item => new DishBasketDto
            {
                Id = item.DishId,
                Name = item.Name,
                Price = item.Price,
                TotalPrice = item.Price * item.Amount,
                Amount = item.Amount,
                Image = item.Image
            }).ToList();

            return dishBasketDtos;
        }

        public async Task<DishBasketDto?> AddDishToBasketAsync(Guid userId, Guid dishId)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

            // Fetch the user's active basket (where DeleteDateTime is null)
            var basket = await _context.Baskets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.DeleteDateTime == null);

            if (basket == null)
            {
                // Create a new basket if none exists
                basket = new Basket
                {
                    UserId = userId,
                    CreateDateTime = DateTime.UtcNow,
                    ModifyDateTime = DateTime.UtcNow
                };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
            }

            // Add the dish to the basket
            var existingBasketItem = basket.Items
                .FirstOrDefault(item => item.DishId == dishId);

            if (existingBasketItem != null)
            {
                existingBasketItem.Amount++;
                existingBasketItem.ModifyDateTime = DateTime.UtcNow;
            }
            else
            {
                var newBasketItem = new BasketItem
                {
                    BasketId = basket.Id,
                    DishId = dishId,
                    Name = dish.Name,
                    Price = dish.Price,
                    Amount = 1,
                    CreateDateTime = DateTime.UtcNow,
                    ModifyDateTime = DateTime.UtcNow
                };
                basket.Items.Add(newBasketItem);
            }

            await _context.SaveChangesAsync();

            var basketItem = basket.Items.First(item => item.DishId == dishId);

            return new DishBasketDto
            {
                Id = basketItem.Id,
                Name = basketItem.Name,
                Price = basketItem.Price,
                TotalPrice = basketItem.Price * basketItem.Amount,
                Amount = basketItem.Amount,
                Image = basketItem.Image
            };
        }

        public async Task<bool> UpdateDishQuantityInBasketAsync(Guid dishId, bool increase)
        {
            _logger.LogInformation($"Attempting to update quantity for Dish with ID: {dishId}, Increase: {increase}");

            var dish = await _context.BasketItems.FirstOrDefaultAsync(b => b.DishId == dishId);

            if (dish == null)
            {
                _logger.LogWarning($"Dish with ID: {dishId} not found.");
                return false;
            }

            if (increase)
            {
                dish.Amount += 1;
                _logger.LogInformation($"Increased quantity for Dish: {dish.Name}. New Amount: {dish.Amount}");
            }
            else
            {
                dish.Amount -= 1;
                _logger.LogInformation($"Decreased quantity for Dish: {dish.Name}. New Amount: {dish.Amount}");

                if (dish.Amount <= 0)
                {
                    _logger.LogInformation($"Quantity for Dish: {dish.Name} is zero. Removing from basket.");
                    _context.BasketItems.Remove(dish);
                }
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Dish quantity updated successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to update dish quantity. SaveChangesAsync did not persist changes.");
                return false;
            }
        }

        public async Task<bool> ClearBasketAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Clearing basket for user: {userId}");

                // Fetch the user's current active basket
                var basket = await _context.Baskets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.DeleteDateTime == null);

                if (basket == null)
                {
                    _logger.LogWarning($"No active basket found for user: {userId}");
                    return false;
                }

                // Mark the current basket as deleted
                basket.DeleteDateTime = DateTime.UtcNow;

                // Remove all items from the basket
                _context.BasketItems.RemoveRange(basket.Items);

                // Create a new basket for the user
                var newBasket = new Basket
                {
                    UserId = userId,
                    CreateDateTime = DateTime.UtcNow,
                    ModifyDateTime = DateTime.UtcNow
                };
                _context.Baskets.Add(newBasket);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Basket cleared and new basket created successfully for user: {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while clearing the basket for user: {userId}");
                throw;
            }
        }
    }
}
