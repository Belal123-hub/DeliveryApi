using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
    public class BasketController : ControllerBase
    {
        private readonly ILogger<BasketController> _logger;
        private readonly IBasketService _basketService;
        public BasketController(IBasketService basketService, ILogger<BasketController> logger)
        {
            _basketService = basketService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Get user cart.")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(DishBasketDto))]
        public async Task<IActionResult> GetAllDishesInBasket()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID" });
            }

            var cart = await _basketService.GetAllDishesInBasketAsync(userId);

            if (cart == null || !cart.Any())
                return Ok(new List<DishBasketDto>());

            return Ok(cart);
        }

        [Authorize]
        [HttpPost("Dish/{DishID}")]
        [SwaggerOperation(Summary = "Add dish to cart.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.")]
        [Produces("application/json")]
        public async Task<IActionResult> AddDishToBasket( AddDishToBasketDto addDishToBasketDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID" });
            }

            var result = await _basketService.AddDishToBasketAsync(Guid.Parse(userIdClaim), addDishToBasketDto.DishId);

            if (result == null)
                return BadRequest(new { Message = "Failed to add dish to basket" });

            return Ok("Success");
        }

        [Authorize]
        [HttpDelete("Dish/{DishId}")]
        [SwaggerOperation(Summary = "Decrease the number of dishes in cart (if increase is true) (if increase false remove it completely).")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.")]
        public async Task<IActionResult> UpdateOrRemoveDishFromBasket(Guid DishId, [FromQuery] bool increase)
        {
            if (DishId == Guid.Empty)
            {
                _logger.LogWarning("Invalid DishId provided.");
                return BadRequest(new { Message = "Invalid DishId" });
            }

            _logger.LogInformation($"Received request to update Dish with ID: {DishId}. Increase: {increase}");
            var success = await _basketService.UpdateDishQuantityInBasketAsync(DishId, increase);

            if (success)
            {
                _logger.LogInformation("Dish updated successfully.");
                return Ok(new { Message = "Success" });
            }
            else
            {
                _logger.LogWarning("Dish not found or could not be updated.");
                return NotFound(new { Message = "Dish not found or could not be updated" });
            }
        }
    }
}