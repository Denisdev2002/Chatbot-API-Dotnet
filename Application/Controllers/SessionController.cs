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
        private readonly IDatabase _redis;

        public SessionController(ISessionApplication sessionApplication, ILogger<SessionController> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _sessionApplication = sessionApplication;
            _logger = logger;
            _redis = connectionMultiplexer.GetDatabase();
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            try
            {
                var cacheKey = "allSessions";
                var cachedSessions = await _redis.StringGetAsync("allSessions");
                if (!cachedSessions.HasValue)
                {
                    _logger.LogInformation("Cache miss: {CacheKey}", cacheKey);
                }
                else
                {
                    _logger.LogInformation("Cache hit: {CacheKey}", cacheKey);
                }

                if (!cachedSessions.IsNullOrEmpty)
                {
                    var sessions = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<SessionViewModel>>(cachedSessions);
                    return Ok(sessions);
                }

                var sessionsFromDb = await _sessionApplication.GetSessions();
                var sessionsJson = System.Text.Json.JsonSerializer.Serialize(sessionsFromDb);
                await _redis.StringSetAsync("allSessions", sessionsJson, TimeSpan.FromMinutes(10));

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

                // Invalidate cache
                await _redis.KeyDeleteAsync("allSessions");

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

                // Invalidate cache
                await _redis.KeyDeleteAsync("allSessions");

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

                // Invalidate cache
                await _redis.KeyDeleteAsync("allSessions");

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
                var cachedSession = await _redis.StringGetAsync($"session:{id}");
                if (!cachedSession.IsNullOrEmpty)
                {
                    var session = System.Text.Json.JsonSerializer.Deserialize<SessionViewModel>(cachedSession);
                    return Ok(session);
                }

                var sessionFromDb = await _sessionApplication.GetSessionId(id);
                if (sessionFromDb == null)
                {
                    return NotFound($"Sessão com Id: {id} não encontrada.");
                }

                var sessionJson = System.Text.Json.JsonSerializer.Serialize(sessionFromDb);
                await _redis.StringSetAsync($"session:{id}", sessionJson, TimeSpan.FromMinutes(10));

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
