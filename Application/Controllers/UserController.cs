using Application.Service.Interfaces;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserApplication userApplication, ILogger<UserController> logger)
        {
            _userApplication = userApplication;
            _logger = logger;
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


        //[Authorize(Roles = "Admin")]
        [HttpGet("Email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userApplication.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error when searching for user with email: {email}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "There was an error when searching for the user.");
            }
        }
    }
}
