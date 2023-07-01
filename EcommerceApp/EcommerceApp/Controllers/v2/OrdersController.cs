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
    /// OrdersController handles order-related operations. 
    /// It uses the MediatR library for handling commands and queries.
    /// </summary>
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// The constructor takes `IMediator` which allows the controller to use MediatR for handling commands and queries.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// HTTP GET endpoint that retrieves all the orders of a user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();

            try
            {
                var orders = await _mediator.Send(query);
                _logger.LogInformation("Controller: Orders retrieved successfully.");
                return Ok(orders);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP GET endpoint that retrieves an order by its orderId.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrderById(Guid orderId)
        {
            var query = new GetOrderByIdQuery { OrderId = orderId };

            try
            {
                var order = await _mediator.Send(query);

                if (order == null)
                {
                    _logger.LogInformation("Controller: Error retrieving order.");
                    return NotFound();
                }
                _logger.LogInformation("Controller: Order retrieved successfully.");
                return Ok(order);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP PUT endpoint for updating an order.
        /// </summary>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateOrder(OrderDto orderDto)
        {
            var command = new UpdateOrderCommand { Order = orderDto };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: Order updated successfully.");
                return Ok("Order updated successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP DELETE endpoint for deleting an order by its orderId.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            var command = new DeleteOrderCommand { OrderId = orderId };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: Order deleted successfully.");
                return Ok("Order deleted successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
