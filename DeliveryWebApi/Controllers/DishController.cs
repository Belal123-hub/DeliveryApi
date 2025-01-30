using BLL.Services;
using DTO;
using DTO.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishesService _dishesService;
        private readonly ILogger<OrderController> _logger;
        public DishController(IDishesService dishesService, ILogger<OrderController> logger)
        {
            _dishesService = dishesService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get list of dishes(menue).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(DishPagedListDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
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

        [Authorize]
        [HttpGet("{id}/rating/check")]
        [SwaggerOperation(Summary = "Check if the user can rate the dish.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> CanRateDish(Guid id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "Id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found.");
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var canRate = await _dishesService.CanUserRateDishAsync(userId, id);

                return Ok(canRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while checking if the user can rate the dish.");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "An error occurred while processing your request." });
            }
        }

        [Authorize]
        [HttpPost("{id}/rating")]
        [SwaggerOperation(Summary = "Set a rating for a dish.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> SetDishRating(Guid id, [FromQuery] int ratingScore)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "Id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found.");
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var canRate = await _dishesService.CanUserRateDishAsync(userId, id);
                if (!canRate)
                {
                    return Forbid();
                }
                var result = await _dishesService.SetDishRatingAsync(userId, id, ratingScore);
                if (!result)
                {
                    return BadRequest("Unable to set the rating.");
                }

                return Ok("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while setting the dish rating.");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "An error occurred while processing your request." });
            }
        }
    }
}

