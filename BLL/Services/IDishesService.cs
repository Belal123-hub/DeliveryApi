using DAL.Data;
using DTO;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;


namespace BLL.Services
{
    public interface IDishesService
    {
        Task<DishPagedListDto> GetAllDishesAsync(int page, int size, DishSorting? sorting, bool? vegetarian, DishCategory? category);
        Task<DishDto?> GetDishByIdAsync(Guid id);
        Task<bool> CanUserRateDishAsync(Guid userId, Guid dishId);
        Task<bool> SetDishRatingAsync(Guid userId, Guid dishId, int ratingScore); // New method

    }
    public class DishService : IDishesService
    {
        private ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DishDto?> GetDishByIdAsync(Guid id)
        {
            // Detach any existing entity with the same ID
            var existingEntity = _context.Dishes.Local.FirstOrDefault(d => d.Id == id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            // Fetch the dish from the database
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == id);
            if (dish == null)
                return null;

            return new DishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Image = dish.Image,
                Vegetarian = dish.IsVegetarian,
                Rating = dish.Rating,
                Category = (DTO.Enums.DishCategory?)dish.Category
            };

        }

        public async Task<DishPagedListDto> GetAllDishesAsync(int page, int size, DishSorting? sorting, bool? vegetarian, DishCategory? category)
        {
            var query = _context.Dishes.AsQueryable();

            // filter by category
            if (category.HasValue)
                query = query.Where(d => d.Category == category.Value);

            // filter by vegeterian 
            if (vegetarian.HasValue)
                query = query.Where(d => d.IsVegetarian == vegetarian.Value);

            // Apply sorting
            query = sorting switch
            {
                DishSorting.NameAsc => query.OrderBy(d => d.Name),
                DishSorting.NameDesc => query.OrderByDescending(d => d.Name),
                DishSorting.PriceAsc => query.OrderBy(d => d.Price),
                DishSorting.PriceDesc => query.OrderByDescending(d => d.Price),
                DishSorting.RatingAsc => query.OrderBy(d => d.Rating),
                DishSorting.RatingDesc => query.OrderByDescending(d => d.Rating),
                _ => query
            };

            // Pagination
            var totalItems = await query.CountAsync();
            var dishes = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var dishDtos = dishes.Select(d => new DishDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price,
                Image = d.Image,
                Vegetarian = d.IsVegetarian,
                Rating = d.Rating,
            }).ToList();

            return new DishPagedListDto
            {
                Dishes = dishDtos,
                Paginatin = new PageInfoModel
                {
                    Size = size,
                    Count = totalItems,
                    Current = page,
                }
            };
        }

        public async Task<bool> CanUserRateDishAsync(Guid userId, Guid dishId)
        {
            // Check if the dish exists
            var dishExists = await _context.Dishes.AnyAsync(d => d.Id == dishId);
            if (!dishExists)
            {
                return false; // Dish does not exist
            }

            // Check if the user has already rated the dish
            var hasRated = await _context.DishRatings.AnyAsync(r => r.UserId == userId && r.DishId == dishId);
            if (hasRated)
            {
                return false; // User has already rated the dish
            }

            // Optional: Check if the user has ordered the dish before (if required)
            var hasOrdered = await _context.Orders
                .AnyAsync(o => o.UserId == userId && o.Items.Any(oi => oi.DishId == dishId));

            return hasOrdered; // User can rate the dish if they have ordered it
        }

        public async Task<bool> SetDishRatingAsync(Guid userId, Guid dishId, int ratingScore)
        {
            if (ratingScore < 1 || ratingScore > 5)
            {
                return false;
            }

            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish == null)
            {
                throw new InvalidOperationException("Dish not found.");
            }

            var existingRating = await _context.DishRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.DishId == dishId);

            if (existingRating != null)
            {
                existingRating.RatingScore = ratingScore;
            }
            else
            {
                var newRating = new DishRating
                {
                    UserId = userId,
                    DishId = dishId,
                    RatingScore = ratingScore
                };
                _context.DishRatings.Add(newRating);
            }

            // Compute new average rating from DishRatings table, including the new rating
            var ratings = await _context.DishRatings
                .Where(r => r.DishId == dishId)
                .Select(r => r.RatingScore)
                .ToListAsync();

            dish.Rating = ratings.Any() ? ratings.Average() : ratingScore; // Ensure update

            _context.Dishes.Update(dish); // Ensure EF Core tracks the change
            await _context.SaveChangesAsync();
            return true;
        }




    }
}
