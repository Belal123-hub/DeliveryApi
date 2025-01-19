using BLL.Services;
using DTO;
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
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
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
    }
}