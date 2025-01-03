using BLL.Services;
using DTO.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController: ControllerBase
    {
        private readonly IDishesService _dishesService;

        public DishController(IDishesService dishesService)
        {
            _dishesService = dishesService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDishById(int id) 
        {
            var dish = await _dishesService.GetDishByIdAsync(id);
            if (dish == null) 
                return NotFound(new { Message = "Dish not found" });

            return Ok(dish);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDishes(
     [FromQuery] int page = 1,
     [FromQuery] int size = 10,
     [FromQuery] DishSorting? sorting = null,
     [FromQuery] bool? vegetarian = null,
     [FromQuery] DishCategory? category = null)
        {
            var dishes = await _dishesService.GetAllDishesAsync(page, size, sorting, vegetarian,category);
            return Ok(dishes);
        }

    }
}
