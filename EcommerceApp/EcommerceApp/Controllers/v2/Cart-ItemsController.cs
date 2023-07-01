using EcommerceApp.Commands;
using EcommerceApp.DTOs;
using EcommerceApp.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;

namespace EcommerceApp.Controllers.v2
{
    /// <summary>
    /// Cart_ItemsController handles cart item-related operations. 
    /// It uses the MediatR library for handling commands and queries.
    /// </summary>
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class Cart_ItemsController : ControllerBase
    {
        private readonly ILogger<Cart_ItemsController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// The constructor takes `IMediator` which allows the controller to use MediatR for handling commands and queries.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        public Cart_ItemsController(IMediator mediator, ILogger<Cart_ItemsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// HTTP GET endpoint that retrieves all the cart items of a pending order.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllCartItems()
        {
            var query = new CartItemQuery();
            try
            {
                var cartItems = await _mediator.Send(query);
                _logger.LogInformation("Controller: Cart items retrieved successfully.");
                return Ok(cartItems);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new cart item.
        /// </summary>
        /// <param name="cartItemDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddCartItem(CartItemDto cartItemDto)
        {
            var command = new AddCartItemCommand { CartItem = cartItemDto };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: Cart item added successfully.");
                return Ok("Cart item added successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP PUT endpoint for updating a cart item.
        /// </summary>
        /// <param name="cartItemDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateCartItem(CartItemDto cartItemDto)
        {
            var command = new UpdateCartItemCommand { CartItem = cartItemDto };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: Cart item updated successfully.");
                return Ok("Cart item updated successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP DELETE endpoint for deleting a cart item by its cartItemId.
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        [HttpDelete("{cartItemId}")]
        public async Task<ActionResult> DeleteCartItem(Guid cartItemId)
        {
            var command = new DeleteCartItemCommand { CartItemId = cartItemId };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: Cart item deleted successfully.");
                return Ok("Cart item deleted successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
