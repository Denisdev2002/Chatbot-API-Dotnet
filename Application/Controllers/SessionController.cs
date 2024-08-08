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
    public class SessionController : ControllerBase
    {
        private readonly ISessionApplication _sessionApplication;
        private readonly ILogger<SessionController> _logger;

        public SessionController(
            ISessionApplication sessionApplication, 
            ILogger<SessionController> logger)
        {
            _sessionApplication = sessionApplication;
            _logger = logger;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            try
            {
                var sessionsFromDb = await _sessionApplication.GetSessions();
                return Ok(sessionsFromDb);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter as sessões.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao obter as sessões.");
            }
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] SessionViewModel sessionViewModel)
        {
            try
            {
                var session = await _sessionApplication.InsertSession(sessionViewModel);
                return Ok(session);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar a sessão.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(string id, [FromBody] SessionViewModel sessionViewModel)
        {
            try
            {
                await _sessionApplication.UpdateSession(id, sessionViewModel);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar a sessão com Id: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao atualizar a sessão.");
            }
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(string id)
        {
            try
            {
                await _sessionApplication.DeleteSessionId(id);
                return Ok($"Sessão com o id: {id} excluída com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao excluir a sessão com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSessionById(string id)
        {
            try
            {
                var sessionFromDb = await _sessionApplication.GetSessionId(id);
                if (sessionFromDb == null)
                {
                    return NotFound($"Sessão com Id: {id} não encontrada.");
                }
                return Ok(sessionFromDb);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar a sessão com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar a sessão.");
            }
        }
    }
}
