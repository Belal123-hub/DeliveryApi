using BLL.Services;
using DTO;
using DTO.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishesService _dishesService;

        public DishController(IDishesService dishesService)
        {
            _dishesService = dishesService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get list of dishes(menue).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(DishPagedListDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError,"InternalServerError.",typeof(Response))]
        public async Task<IActionResult> GetAllDishes(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] DishSorting? sorting = null,
            [FromQuery] bool? vegetarian = null,
            [FromQuery] DishCategory? category = null)
        {
            var dishes = await _dishesService.GetAllDishesAsync(page, size, sorting, vegetarian, category);
            return Ok(dishes);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get information about concrete dish.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(DishDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> GetDishById(Guid id)
        {
            var dish = await _dishesService.GetDishByIdAsync(id);
            if (dish == null)
                return NotFound(new Response { Status = "Error", Message = "Dish not found" });

            return Ok(dish);
        }
    }
}

