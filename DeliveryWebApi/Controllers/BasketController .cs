using Backend2024ExampleApp.Controllers;
using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ILogger<BasketController> _logger;

        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService, ILogger<BasketController> logger)
        {
            _basketService = basketService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Check for null logger
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllDishesInBasket()
        {
            // Extract the userId from the authenticated user's claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID" });
            }

            var cart = await _basketService.GetAllDishesInBasketAsync(userId);

            if (cart == null || !cart.Any())
                return NotFound(new { Message = "No dishes in the basket" });

            return Ok(cart);
        }

        [Authorize]
        [HttpPost("Dish/{DishById}")]
        public async Task<IActionResult> AddDishToBasket([FromBody] AddDishToBasketDto addDishToBasketDto)
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


        // DELETE: api/Basket/Clear
        [Authorize]
        [HttpDelete("Dish/{DishId}")]
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
