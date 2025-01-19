using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IBasketService _basketService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IBasketService basketService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _basketService = basketService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderInfoDto>>> GetOrders()
        {
            try
            {
                _logger.LogInformation("Fetching all orders.");

                var orders = await _orderService.GetOrdersAsync();

                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found.");
                    return NotFound("No orders found.");
                }

                _logger.LogInformation("Successfully fetched {Count} orders.", orders.Count());
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching orders.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid order creation data provided.");
                return BadRequest(new { Message = "Invalid input data" });
            }

            // Extract the userId from the authenticated user's claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID" });
            }

            try
            {
                // Create the order from the current basket
                var order = await _orderService.CreateOrderFromBasketAsync(userId, orderCreateDto);

                if (order == null)
                {
                    return BadRequest(new { Message = "Failed to create order or basket is empty" });
                }

                // Clear the basket after creating the order
                var isBasketCleared = await _basketService.ClearBasketAsync(userId);

                if (!isBasketCleared)
                {
                    _logger.LogWarning("Basket was not cleared after order creation.");
                }

                return Ok(new { Message = "Order created successfully", OrderId = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order.");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }
}