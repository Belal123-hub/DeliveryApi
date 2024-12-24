using DAL.Data;
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
        Task<IEnumerable<Dish>> GetAllDishesAsync();
        
    }
    public class DishService : IDishesService 
    {
        private ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllDishesAsync()
        {
            return await _context.Dishes.ToListAsync();
        }
    }
     
}
