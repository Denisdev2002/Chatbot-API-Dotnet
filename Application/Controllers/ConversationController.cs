using Application.Service.Interfaces;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationApplication _conversationApplication;
        private readonly ILogger<ConversationController> _logger;
        private readonly IDatabase _redis;

        public ConversationController(
            IConversationApplication conversationApplication,
            ILogger<ConversationController> logger,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _conversationApplication = conversationApplication;
            _logger = logger;
            _redis = connectionMultiplexer.GetDatabase();
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var cacheKey = "all_conversations";
                var cachedData = await _redis.StringGetAsync(cacheKey);

                if (cachedData.HasValue)
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                    var conversations = JsonSerializer.Deserialize<IEnumerable<ConversationViewModel>>(cachedData);
                    return Ok(conversations);
                }

                _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
                var conversationsFromService = await _conversationApplication.GetAllConversations();
                var serializedData = JsonSerializer.Serialize(conversationsFromService);
                await _redis.StringSetAsync(cacheKey, serializedData, TimeSpan.FromHours(1));
                return Ok(conversationsFromService);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter as conversas.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao obter as conversas.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation([FromBody] ConversationViewModel conversationViewModel)
        {
            try
            {
                var conversation = await _conversationApplication.CreateConversation(conversationViewModel);

                await UpdateConversationsCacheAsync();

                return Ok("Conversa inserida com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao inserir a conversa.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConversation(string id, [FromBody] ConversationViewModel conversationViewModel)
        {
            try
            {
                await _conversationApplication.UpdateConversation(id, conversationViewModel);

                // Atualizar cache para garantir que os dados mais recentes sejam obtidos
                await UpdateConversationsCacheAsync();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar a conversa com Id: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao atualizar a conversa.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversation(string id)
        {
            try
            {
                await _conversationApplication.DeleteConversation(id);

                // Atualizar cache para garantir que os dados mais recentes sejam obtidos
                await UpdateConversationsCacheAsync();

                return Ok($"Conversa com o id: {id} excluída com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao excluir a conversa com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationById(string id)
        {
            try
            {
                var cacheKey = $"conversation_{id}";
                var cachedData = await _redis.StringGetAsync(cacheKey);

                if (cachedData.HasValue)
                {
                    var conversation = JsonSerializer.Deserialize<ConversationViewModel>(cachedData);
                    return Ok(conversation);
                }

                var conversationFromService = await _conversationApplication.GetByIdConversation(id);
                if (conversationFromService == null)
                {
                    return NotFound($"Conversation com Id: {id} não encontrada.");
                }

                var serializedData = JsonSerializer.Serialize(conversationFromService);
                await _redis.StringSetAsync(cacheKey, serializedData, TimeSpan.FromHours(1));

                return Ok(conversationFromService);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar a conversa com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar a conversa.");
            }
        }

        private async Task UpdateConversationsCacheAsync()
        {
            var cacheKey = "all_conversations";
            var conversationsFromService = await _conversationApplication.GetAllConversations();
            var serializedData = JsonSerializer.Serialize(conversationsFromService);
            await _redis.StringSetAsync(cacheKey, serializedData, TimeSpan.FromHours(1));
        }
    }
}
