using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDishesInBasket()
        {
            var cart = await _basketService.GetAllDishesInBasketAsync();

            if (cart == null || !cart.Any())
                return NotFound(new { Message = "No dishes in the basket" });

            return Ok(cart);
        }

        [HttpPost("Dish/{DishById}")]
        public async Task<IActionResult> AddDishToBasket([FromBody] AddDishToBasketRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Invalid DishId" });
            }

            var success = await _basketService.AddDishToBasketByIdAsync(request.DishId);

            if (success)
            {
                return Ok(new { Message = "Dish added to basket successfully" });
            }
            else
            {
                return NotFound(new { Message = "Dish not found" });
            }
        }

        [HttpDelete("Dish/{DishId}")]
        public async Task<IActionResult> UpdateOrRemoveDishFromBasket(Guid DishId, [FromQuery] bool increase)
        {
            if (DishId == Guid.Empty)
            {
                Console.WriteLine("Invalid DishId provided.");
                return BadRequest(new { Message = "Invalid DishId" });
            }

            Console.WriteLine($"Received request to update Dish with ID: {DishId}. Increase: {increase}");
            var success = await _basketService.UpdateDishQuantityInBasketAsync(DishId, increase);

            if (success)
            {
                Console.WriteLine("Dish updated successfully.");
                return Ok(new { Message = "Dish updated successfully" });
            }
            else
            {
                Console.WriteLine("Dish not found or could not be updated.");
                return NotFound(new { Message = "Dish not found or could not be updated" });
            }
        }

    }
}
