// BLL/Services/OrderService.cs
using DTO;
using DAL.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DTO.Enums;
using Microsoft.AspNetCore.Http;

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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(ILogger<OrderService> logger, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation($"Fetching order with ID: {orderId}");

                // Fetch the order from the database
                var order = await _context.Orders
                    .Include(o => o.Items)
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
                    DeliveryTime = order.DeliveryTime.ToString("o"),
                    OrderTime = order.OrderTime.ToString("o"),
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
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(c => c.Type == "Id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Unauthorized access: Missing or invalid user ID.");
                    return Enumerable.Empty<OrderInfoDto>();
                }

                _logger.LogInformation($"Fetching orders for user ID: {userId}");
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .ToListAsync();

                if (!orders.Any())
                {
                    _logger.LogWarning($"No orders found for user ID: {userId}");
                    return Enumerable.Empty<OrderInfoDto>();
                }

                var ordersDto = orders.Select(o => new OrderInfoDto
                {
                    Id = o.Id,
                    DeliveryTime = o.DeliveryTime,
                    OrderTime = o.OrderTime,
                    Status = o.Status,
                    Price = o.Price
                });

                _logger.LogInformation($"Successfully fetched {ordersDto.Count()} orders for user ID: {userId}");
                return ordersDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching orders.");
                throw;
            }
        }

        public async Task<OrderDto?> CreateOrderFromBasketAsync(Guid userId, OrderCreateDto orderCreateDto)
        {
            try
            {
                _logger.LogInformation($"Creating order from basket for user: {userId}");

                // Get the current UTC time for order placement
                var orderTime = DateTime.UtcNow;

                // Ensure delivery time is at least 1 hour after order time
                if (orderCreateDto.DeliveryTime <= orderTime.AddHours(1))
                {
                    _logger.LogWarning($"Invalid delivery time for user {userId}. Delivery must be at least 1 hour after order time.");
                    throw new BadHttpRequestException("Delivery time must be at least 1 hour after order time.");
                }
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
                    OrderTime = orderTime,
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
                return new OrderDto
                {
                    Id = order.Id.ToString(),
                    DeliveryTime = order.DeliveryTime.ToString("o"),
                    OrderTime = order.OrderTime.ToString("o"),
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
                if (order.Status == OrderStatus.Delivered)
                {
                    _logger.LogWarning($"Order with ID: {orderId} is already delivered.");
                    return false;
                }
                order.Status = OrderStatus.Delivered;
                order.ModifyDateTime = DateTime.UtcNow;
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