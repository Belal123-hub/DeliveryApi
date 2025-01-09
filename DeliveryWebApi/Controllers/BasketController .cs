using BLL.Services;
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
    }
}
