// BLL/Services/OrderService.cs
using DTO;
using DAL.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using DTO.Enums;

namespace BLL.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderInfoDto>> GetOrdersAsync();
        Task<OrderDto?> CreateOrderFromBasketAsync(Guid userId, OrderCreateDto orderCreateDto);
        Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
        Task<bool> ConfirmOrderDeliveryAsync(Guid orderId); // New method


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

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation($"Fetching order with ID: {orderId}");

                // Fetch the order from the database
                var order = await _context.Orders
                    .Include(o => o.Items) // Include related items
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning($"Order with ID: {orderId} not found.");
                    return null;
                }

                // Map the order to OrderDto
                return new OrderDto
                {
                    Id = order.Id.ToString(),
                    DeliveryTime = order.DeliveryTime.ToString("o"), // ISO 8601 format
                    OrderTime = order.OrderTime.ToString("o"), // ISO 8601 format
                    Status = order.Status,
                    Price = (double)order.Price,
                    Address = order.Address,
                    Dishes = order.Items.Select(item => new DishBasketDto
                    {
                        Id = item.DishId,
                        Name = item.Name,
                        Price = item.Price,
                        TotalPrice = (item.Price * item.Amount),
                        Amount = item.Amount,
                        Image = item.Image
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching order with ID: {orderId}");
                throw;
            }
        }

        public async Task<IEnumerable<OrderInfoDto>> GetOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching orders from the database.");

                // Fetch orders from the database
                var orders = await _context.Orders.ToListAsync();

                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found in the database.");
                    return Enumerable.Empty<OrderInfoDto>();
                }

                // Map the data to DTO
                var ordersDto = orders.Select(o => new OrderInfoDto
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

        public async Task<OrderDto?> CreateOrderFromBasketAsync(Guid userId, OrderCreateDto orderCreateDto)
        {
            try
            {
                _logger.LogInformation($"Creating order from basket for user: {userId}");

                // Fetch the user's current basket
                var basket = await _context.Baskets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.DeleteDateTime == null);

                if (basket == null || !basket.Items.Any())
                {
                    _logger.LogWarning($"No basket or items found for user: {userId}");
                    return null;
                }

                // Create a new order
                var order = new Order
                {
                    UserId = userId,
                    DeliveryTime = orderCreateDto.DeliveryTime, 
                    Address = orderCreateDto.Address,
                    OrderTime = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Price = basket.Items.Sum(item => item.Price * item.Amount),
                    Items = basket.Items.Select(item => new OrderItem
                    {
                        DishId = item.DishId,
                        Name = item.Name,
                        Price = item.Price,
                        Amount = item.Amount,
                        Image = item.Image
                    }).ToList()
                };

                // Add the order to the database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order created successfully for user: {userId}, Order ID: {order.Id}");

                // Map the order to a DTO
                return new OrderDto
                {
                    Id = order.Id.ToString(),
                    DeliveryTime = order.DeliveryTime.ToString("o"), // ISO 8601 format
                    OrderTime = order.OrderTime.ToString("o"), // ISO 8601 format
                    Status = order.Status,
                    Price = (double)order.Price,
                    Address = order.Address,
                    Dishes = order.Items.Select(item => new DishBasketDto
                    {
                        Id = item.DishId,
                        Name = item.Name,
                        Price = item.Price,
                        TotalPrice = (item.Price * item.Amount),
                        Amount = item.Amount,
                        Image = item.Image
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating an order for user: {userId}");
                throw;
            }
        }

        public async Task<bool> ConfirmOrderDeliveryAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation($"Confirming delivery for order with ID: {orderId}");

                // Fetch the order from the database
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning($"Order with ID: {orderId} not found.");
                    return false;
                }

                // Check if the order is already delivered
                if (order.Status == OrderStatus.Delivered)
                {
                    _logger.LogWarning($"Order with ID: {orderId} is already delivered.");
                    return false;
                }

                // Update the order status to Delivered
                order.Status = OrderStatus.Delivered;
                order.ModifyDateTime = DateTime.UtcNow; // Update the modification timestamp

                // Save changes to the database
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully confirmed delivery for order with ID: {orderId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while confirming delivery for order with ID: {orderId}");
                throw;
            }
        }
    }
}