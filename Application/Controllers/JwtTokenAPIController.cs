using Application.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JwtTokenAPI : Controller
    {
        private readonly IJwtTokenApplicationAPI _jwtTokenApplicationAPI;
        private readonly ILogger<JwtTokenAPI> _logger;

        public JwtTokenAPI(
            IJwtTokenApplicationAPI jwtTokenApplicationAPI, 
            ILogger<JwtTokenAPI> logger
            )
        {
            _jwtTokenApplicationAPI = jwtTokenApplicationAPI;
            _logger = logger;
        }

        [HttpPut]
        public async Task<IActionResult> GetKeySecret()
        {
            try
            {
                var conversationsFromService = await _jwtTokenApplicationAPI.GetKeySecretToken();
                return Ok(conversationsFromService);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter a key secret.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao obter a key secret.");
            }
        }
    }
}
