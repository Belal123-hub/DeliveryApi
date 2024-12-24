using DAL.Data;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IDishesService
    {
        Task<DishPagedListDto> GetAllDishesAsync(int page, int size, bool? vegetarian);

    }
    public class DishService : IDishesService
    {
        private ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DishPagedListDto> GetAllDishesAsync(int page, int size, bool? vegetarian)
        {
            var query = _context.Dishes.AsQueryable();

            // filter by vegeterian 
            if (vegetarian.HasValue)
                query = query.Where(d => d.IsVegetarian == vegetarian.Value);

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
