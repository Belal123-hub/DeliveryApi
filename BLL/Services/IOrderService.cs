// BLL/Services/OrderService.cs
using DTO;
using DAL.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace BLL.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly ApplicationDbContext _context;

        public OrderService(ILogger<OrderService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching orders from the database.");

                // Fetch orders from the database
                var orders = await _context.Orders.ToListAsync();

                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found in the database.");
                    return Enumerable.Empty<OrderDto>();
                }

                // Map the data to DTO
                var ordersDto = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    DeliveryTime = o.DeliveryTime,
                    OrderTime = o.OrderTime,
                    Status = o.Status,
                    Price = o.Price
                });

                _logger.LogInformation("Successfully fetched {Count} orders from the database.", ordersDto.Count());
                return ordersDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching orders from the database.");
                throw; // Re-throw the exception to be handled by the controller
            }
        }
    }
}