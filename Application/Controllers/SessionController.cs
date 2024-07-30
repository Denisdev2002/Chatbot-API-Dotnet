using Application.Service.Interfaces;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionApplication _sessionApplication;
        private readonly ILogger<SessionController> _logger;

        public SessionController(ISessionApplication sessionApplication, ILogger<SessionController> logger)
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
                var sessions = await _sessionApplication.GetSessions();
                return Ok(sessions);
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
                _logger.LogError(ex, "Ocorreu um erro ao inserir a sessão.");
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
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar a sessão com ID: {id}");
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
                return Ok($"Sessão {id} excluída com sucesso!");
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
        [HttpPost("external")]
        public async Task<IActionResult> DeleteSessionExternal(string id)
        {
            try
            {
                await _sessionApplication.DeleteSessionIdExternal(id);
                return Ok($"Sessão {id} excluída com sucesso!");
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
        [HttpGet("session/{id}")]
        public async Task<IActionResult> GetSessionById(string id)
        {
            try
            {
                var session = await _sessionApplication.GetSessionId(id);
                if (session == null)
                {
                    return NotFound($"Sessão com ID: {id} não encontrada.");
                }
                return Ok(session);
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

        //[Authorize]
        [HttpGet("session/user/{email}")]
        public async Task<IActionResult> GetSessionByUser(string email)
        {
            try
            {
                var sessions = await _sessionApplication.GetSessionUser(email);
                if (sessions == null)
                {
                    return NotFound($"Sessão com email: {email} não encontrada.");
                }
                return Ok(sessions);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar a sessão com email: {email}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar a sessão.");
            }
        }

        //[Authorize]
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateSession(string id)
        {
            try
            {
                await _sessionApplication.ActivateSession(id);
                return Ok($"Sessão {id} ativada com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao ativar a sessão com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao ativar a sessão.");
            }
        }

        //[Authorize]
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateSession(string id)
        {
            try
            {
                await _sessionApplication.DeactivateSession(id);
                return Ok($"Sessão {id} desativada com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao desativar a sessão com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao desativar a sessão.");
            }
        }
    }

}