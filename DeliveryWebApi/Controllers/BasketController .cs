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
    }
}
