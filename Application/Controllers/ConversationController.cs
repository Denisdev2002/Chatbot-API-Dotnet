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
                var conversationsFromService = await _conversationApplication.GetAllConversations();
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
                var conversationFromService = await _conversationApplication.GetByIdConversation(id);
                if (conversationFromService == null)
                {
                    return NotFound($"Conversation com Id: {id} não encontrada.");
                }
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
    }
}
