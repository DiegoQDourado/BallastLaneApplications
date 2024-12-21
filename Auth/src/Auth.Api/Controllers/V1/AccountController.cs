using Auth.Api.Examples.Requests;
using Auth.Api.Models;
using SharedKernel.Enums;
using Auth.Business.Models;
using SharedKernel.Notifications;
using Auth.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Auth.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly INotification _notification;

        public AccountController(IUserService userService, INotification notification)
        {
            _userService = userService;
            _notification = notification;
        }


        [HttpPost("register")]
        [SwaggerRequestExample(typeof(UserModel), typeof(UserModelRequestExample))]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register(UserModel user)
        {
            await _userService.CreateAsync(user);

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            if (_notification.Any())
            {
                return BadRequest(new ResponseError(_notification.GetSummary()));
            }

            return Created();
        }


        [HttpPost("login")]
        [SwaggerRequestExample(typeof(UserLogin), typeof(UserLoginRequestExample))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var token = await _userService.LoginAsync(userLogin.UserName, userLogin.Password);

            if (_notification.NotificationType is NotificationType.Unexpected)
            {
                return StatusCode(500, new ResponseError(_notification.GetSummary()));
            }

            if (_notification.Any())
            {
                return BadRequest(new ResponseError(_notification.GetSummary()));
            }

            return Ok(new { Token = token });
        }
    }
}
