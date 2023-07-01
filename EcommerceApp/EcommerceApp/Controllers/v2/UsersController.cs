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
    /// UsersController handles user-related operations. 
    /// It uses the MediatR library for handling commands and queries.
    /// </summary>
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// The constructor takes `IMediator` which allows the controller to use MediatR for handling commands and queries.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// HTTP GET endpoint that retrieves a user by their userId.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserById(Guid userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };

            try
            {
                var user = await _mediator.Send(query);

                if (user == null)
                {
                    _logger.LogError("Controller: Error retrieving user.");
                    return NotFound();
                }
                _logger.LogInformation("Controller: User retrieved successfully.");
                return Ok(user);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: { ex.Message }");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddUser(UserDto userDto)
        {
            var command = new AddUserCommand { User = userDto };

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Controller: User added successfully.");
                return Ok("User added successfully.");
            }
            catch (BadRequestException ex)
            {
                _logger.LogError($"Controller: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}