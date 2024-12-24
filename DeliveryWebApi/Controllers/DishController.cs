using BLL.Services;
using DAL.Enums;
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

        [HttpGet]
        public async Task<IActionResult> GetAllDishes(
     [FromQuery] int page = 1,
     [FromQuery] int size = 10,
     [FromQuery] bool? vegetarian = null,
     [FromQuery] DishCategory? category = null)
        {
            var dishes = await _dishesService.GetAllDishesAsync(page, size, vegetarian,category);
            return Ok(dishes);
        }


    }
}
