using DAL;
using DAL.Data;
using DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<BasketService> _logger;

        // Modify the constructor to accept ILogger<BasketService>
        public BasketService(ApplicationDbContext context, ILogger<BasketService> logger)
        {
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<DishBasketDto>?> GetAllDishesInBasketAsync(Guid userId)
        {
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.DeleteDateTime == null);

            if (basket == null || !basket.Items.Any())
                return null;
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
            if (dish == null)
            {
                _logger.LogWarning($"Dish with ID {dishId} not found.");
                return null;
            }
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.DeleteDateTime == null);

            if (basket == null)
            {
                // Create a new basket if none exists
                basket = new Basket
                {
                    UserId = userId,
                    CreateDateTime = DateTime.UtcNow,
                    ModifyDateTime = DateTime.UtcNow,
                    Items = new List<BasketItem>() // Initialize Items list
                };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync(); // Ensure basket is saved first
            }
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
                _context.BasketItems.Add(newBasketItem); 
            }

            await _context.SaveChangesAsync();
            var basketItem = basket.Items.First(item => item.DishId == dishId);

            return new DishBasketDto
            {
                Id = basketItem.DishId,
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
            var dishes = await _context.BasketItems
                .Where(b => b.DishId == dishId)
                .ToListAsync();

            if (dishes == null || !dishes.Any())
            {
                _logger.LogWarning($"Dish with ID: {dishId} not found in any basket.");
                return false;
            }

            if (increase)
            {
                // Decrease quantity by 1
                foreach (var dish in dishes)
                {
                    dish.Amount -= 1;
                    dish.ModifyDateTime = DateTime.UtcNow;
                    _logger.LogInformation($"Decreased quantity for Dish: {dish.Name}. New Amount: {dish.Amount}");
                    if (dish.Amount <= 0)
                    {
                        _logger.LogInformation($"Quantity for Dish: {dish.Name} is zero. Removing from basket.");
                        _context.BasketItems.Remove(dish);
                    }
                }
            }
            else
            {
                _logger.LogInformation($"Removing all instances of Dish: {dishes.First().Name} from the basket.");
                _context.BasketItems.RemoveRange(dishes);
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
                basket.DeleteDateTime = DateTime.UtcNow;
                _context.BasketItems.RemoveRange(basket.Items);
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
