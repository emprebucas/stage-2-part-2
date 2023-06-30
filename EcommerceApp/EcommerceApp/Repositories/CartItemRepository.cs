using Dapper;
using EcommerceApp.Data;
using EcommerceApp.Entities;
using EcommerceApp.Interfaces;
using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SendGrid.Helpers.Errors.Model;

namespace EcommerceApp.Repositories
{
    /// <summary>
    /// CartItemRepository provides the implementation for retrieving, adding, updating, and deleting cart items.
    /// </summary>
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ECommerceDbContext _dbContext;
        private readonly HttpContextHelper _httpContextHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CartItemRepository> _logger;

        /// <summary>
        /// The constructor takes the `ECommerceDbContext`, `IConfiguration`, and `HttpContextHelper` objects.
        /// `ECommerceDbContext`: used to interact with the database using Entity Framework Core.
        /// `IConfiguration`: used to retrieve the database connection string.
        /// `HttpContextHelper`: used to retrieve 'x-user-id' in the request header.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="configuration"></param>
        /// <param name="httpContextHelper"></param>
        /// <param name="logger"></param>
        public CartItemRepository(ECommerceDbContext dbContext, IConfiguration configuration, HttpContextHelper httpContextHelper, ILogger<CartItemRepository> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _httpContextHelper = httpContextHelper;
            _logger = logger;
        }

        /// <summary>
        /// GetAllCartItemsAsync retrieves all cart items for the current user.
        /// It uses Dapper to execute SQL queries to retrieve the pending order for the user and the corresponding cart items.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CartItemModel>> GetAllCartItemsAsync()
        {
            var userId = _httpContextHelper.GetUserId();

            var connectionString = _configuration.GetConnectionString("ECommerceDb");
            using var connection = new MySqlConnection(connectionString);

            var orderQuery = "SELECT * FROM Orders WHERE Status = @Status AND UserId = @UserId LIMIT @Limit";
            var order = connection.QuerySingleOrDefault<OrderModel>(orderQuery, new { UserId = userId, Status = 0, Limit = 1 });

            if (order == null)
            {
                _logger.LogError("Error retrieving cart items.");
                throw new BadRequestException("User does not have a pending order.");
            }

            var orderId = order.OrderId;

            var cartQuery = "SELECT * FROM CartItems WHERE OrderId = @OrderId";
            var cartItems = await connection.QueryAsync<CartItemModel>(cartQuery, new { OrderId = orderId });

            _logger.LogInformation("Cart items retrieved successfully.");
            return cartItems.AsList();
        }

        /// <summary>
        /// AddCartItemAsync uses Entity Framework Core to add a new cart item.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task AddCartItemAsync(CartItemEntity cartItem)
        {
            var connectionString = _configuration.GetConnectionString("ECommerceDb");
            using var connection = new MySqlConnection(connectionString);

            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == cartItem.UserId);
            if (!userExists)
            {
                _logger.LogError("Error adding cart item.");
                throw new BadRequestException("Cannot add cart item. User is not found.");
            }

            var order = await _dbContext.Orders.FindAsync(cartItem.OrderId);
            if (order == null)
            {
                var userHasPendingOrder = await _dbContext.Orders.AnyAsync(o => o.UserId == cartItem.UserId && o.Status == OrderStatusEntity.Pending);

                if (userHasPendingOrder)
                {
                    _logger.LogError("Error adding cart item.");
                    throw new BadRequestException("Cannot add cart item. User already has a pending order.");
                }
                order = new OrderEntity
                {
                    OrderId = cartItem.OrderId,
                    UserId = cartItem.UserId,
                    Status = OrderStatusEntity.Pending
                };

                _dbContext.Orders.Add(order);

            }
            else if (order.Status != OrderStatusEntity.Pending)
            {
                _logger.LogError("Error adding cart item.");
                throw new BadRequestException("Cannot add cart item to the order. Order is already processed or cancelled.");
            }
            else if (order != null)
            {
                var orderExistsForOtherUser = await _dbContext.Orders.AnyAsync(o => o.UserId != cartItem.UserId && o.OrderId == cartItem.OrderId);

                if (orderExistsForOtherUser)
                {
                    _logger.LogError("Error adding cart item.");
                    throw new BadRequestException("Cannot add cart item. Order already exists.");
                }
            }

            var cartItemExists = await _dbContext.CartItems.AnyAsync(c => c.CartItemId == cartItem.CartItemId);

            if (cartItemExists)
            {
                _logger.LogError("Error adding cart item.");
                throw new BadRequestException("Cannot add cart item. Cart item already exists.");
            }

            _dbContext.CartItems.Add(cartItem);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Cart item added successfully.");
        }

        /// <summary>
        /// UpdateCartItemAsync uses Entity Framework Core to update a cart item.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task UpdateCartItemAsync(CartItemEntity cartItem)
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == cartItem.UserId);
            if (!userExists)
            {
                _logger.LogError("Error updating cart item.");
                throw new BadRequestException("Cannot update cart item. User is not found.");
            }

            var orderExists = await _dbContext.Orders.AnyAsync(o => o.OrderId == cartItem.OrderId && o.UserId == cartItem.UserId);
            if (!orderExists)
            {
                _logger.LogError("Error updating cart item.");
                throw new BadRequestException("Cannot update cart item. Order is not found for user.");
            }

            var cartItemExists = await _dbContext.CartItems.AnyAsync(c => c.CartItemId == cartItem.CartItemId && c.OrderId == cartItem.OrderId);
            if (!cartItemExists)
            {
                _logger.LogError("Error updating cart item.");
                throw new BadRequestException("Cannot update cart item. Cart item is not found for user.");
            }

            _dbContext.CartItems.Update(cartItem);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Cart item updated successfully.");
        }

        /// <summary>
        /// DeleteCartItemAsync uses Entity Framework Core to delete a cart item.
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task DeleteCartItemAsync(Guid cartItemId)
        {
            var cartItem = await _dbContext.CartItems.FindAsync(cartItemId);
            
            if (cartItem == null)
            {
                _logger.LogError("Error deleting cart item.");
                throw new BadRequestException("Cannot delete cart item. Cart item is not found.");
            }

            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Cart item deleted successfully.");
        }

    }

}
