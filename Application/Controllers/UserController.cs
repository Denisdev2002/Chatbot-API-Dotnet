using Application.Service.Interfaces;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;

namespace Application.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;
        private readonly ILogger<UserController> _logger;
        private readonly IDatabase _redis;

        public UserController(IUserApplication userApplication, ILogger<UserController> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _userApplication = userApplication;
            _logger = logger;
            _redis = connectionMultiplexer.GetDatabase();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            try
            {
                var tokenDto = await _userApplication.LoginAsync(loginViewModel);
                if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken))
                {
                    return Unauthorized("Failed to obtain token.");
                }
                return Ok(tokenDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                return BadRequest("Incorrect email or password.");
            }
        }
    }
}
