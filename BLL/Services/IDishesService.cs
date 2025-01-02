using DAL.Data;
using DTO;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;


namespace BLL.Services
{
    public interface IDishesService
    {
        Task<DishPagedListDto> GetAllDishesAsync(int page, int size, DishSorting? sorting, bool? vegetarian, DishCategory? category);
        Task<DishDto?> GetDishByIdAsync(int id);

    }
    public class DishService : IDishesService
    {
        private ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<DishDto?> GetDishByIdAsync(int id) 
        {
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

    }
}
