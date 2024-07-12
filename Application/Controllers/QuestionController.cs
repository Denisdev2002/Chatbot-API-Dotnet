using Application.Service.Interfaces;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionApplication _questionApplication;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(IQuestionApplication questionApplication, ILogger<QuestionController> logger)
        {
            _questionApplication = questionApplication;
            _logger = logger;

        }

        [Authorize]
        [HttpGet("toask/{idQuestion}")]
        public async Task<IActionResult> ToAskForModel(string idQuestion)
        {
            try
            {
                await _questionApplication.ToAsk(idQuestion);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                var questions = await _questionApplication.GetQuestions();
                return Ok(questions);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter as question.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao obter as question.");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionViewModel questionViewModel)
        {
            try
            {
                await _questionApplication.InsertQuestion(questionViewModel);
                return Ok("Question inserida com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao inserir a question.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(string id, [FromBody] QuestionViewModel questionViewModel)
        {
            try
            {


                await _questionApplication.UpdateQuestion(id, questionViewModel);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar a question com Id: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao atualizar a question.");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            try
            {
                await _questionApplication.DeleteQuestion(id);
                return Ok($"Question com o id: {id} excluída com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao excluir a question com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("question/{id}")]
        public async Task<IActionResult> GetQuestionById(string id)
        {
            try
            {
                var question = await _questionApplication.GetQuestionById(id);
                if (question == null)
                {
                    return NotFound($"Question com Id: {id} não encontrada.");
                }
                return Ok(question);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar a question com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar a question.");
            }
        }

        [Authorize]
        [HttpGet("question/session/{idSession}")]
        public async Task<IActionResult> GetQuestionIdSession(string idSession)
        {
            try
            {
                var question = await _questionApplication.GetQuestionByIdSession(idSession);
                if (question == null)
                {
                    return NotFound($"Question com Id Session: {idSession} não encontrada.");
                }
                return Ok(question);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar a question com Id Sessions: {idSession}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar a question.");
            }
        }
    }
}