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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userApplication.GetUsersAsync();
                return Ok(users);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter os usuários.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao obter os usuários.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] UserViewModel userViewModel)
        {
            try
            {
                await _userApplication.InsertUserAsync(userViewModel);
                return Ok(userViewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUser(string email, [FromBody] UserViewModel userViewModel)
        {
            try
            {
                var existingUser = await _userApplication.GetUserByEmailAsync(email);
                if (existingUser == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                await _userApplication.UpdateUserAsync(email, userViewModel);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar o usuário com email: {email}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao atualizar o usuário.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var existingUser = await _userApplication.GetUserByEmailAsync(email);
                if (existingUser == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                await _userApplication.DeleteUserAsync(email);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao deletar o usuário com email: {email}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao deletar o usuário.");
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                var token = await _userApplication.LoginAsync(loginViewModel);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao fazer login.");
                return BadRequest("Email ou senha incorretos.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userApplication.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("Usuário não encontrado.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar usuário com o email: {email}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar o usuário.");
            }
        }
    }
}