using BLL.Services;
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
        public async Task<IActionResult> GetAllDishes()
        {
            var dishes = await _dishesService.GetAllDishesAsync();
            return Ok(dishes);
        }

    }
}
