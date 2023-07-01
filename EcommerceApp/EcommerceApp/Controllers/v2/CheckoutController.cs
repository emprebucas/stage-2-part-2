using EcommerceApp.Commands;
using EcommerceApp.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;

namespace EcommerceApp.Controllers.v2
{
    /// <summary>
    /// CheckoutController handles the checkout operation. 
    /// It uses the MediatR library for handling commands and queries.
    /// </summary>
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ILogger<CheckoutController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// The constructor takes `IMediator` which allows the controller to use MediatR for handling commands and queries.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        public CheckoutController(IMediator mediator, ILogger<CheckoutController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// HTTP POST endpoint for checking out an order.
        /// </summary>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CheckoutOrder(OrderDto orderDto)
        {
            var command = new CheckoutOrderCommand { Order = orderDto };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: Order checked out successfully.");
                return Ok("Order checked out successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }

        }
    }
}
