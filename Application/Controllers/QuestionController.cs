using Application.Service.Interfaces;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

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

        //[Authorize]
        [HttpGet("toask/{idQuestion}")]
        public async Task<IActionResult> ToAskForModel(string idQuestion)
        {
            try
            {

                var responseFromModel = await _questionApplication.ToAsk(idQuestion);

                //var responseJson = JsonConvert.SerializeObject(responseFromModel);
                //await _redis.StringSetAsync(cacheKey, responseJson, TimeSpan.FromMinutes(10));

                return Ok(responseFromModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the question.");
                return StatusCode(500, ex.Message);
            }
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                var questionsFromDb = await _questionApplication.GetQuestions();
                return Ok(questionsFromDb);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter as questões.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao obter as questões.");
            }
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionViewModel questionViewModel)
        {
            try
            {
                var question = await _questionApplication.InsertQuestion(questionViewModel);
                return Ok(question);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao inserir a questão.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(string id, [FromBody] QuestionViewModel questionViewModel)
        {
            try
            {
                await _questionApplication.UpdateQuestion(id, questionViewModel);;
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao atualizar a questão com Id: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao atualizar a questão.");
            }
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            try
            {
                await _questionApplication.DeleteQuestion(id);
                return Ok($"Questão com o id: {id} excluída com sucesso!");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao excluir a questão com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[Authorize]
        [HttpGet("question/{id}")]
        public async Task<IActionResult> GetQuestionById(string id)
        {
            try
            {
                var questionFromDb = await _questionApplication.GetQuestionById(id);
                if (questionFromDb == null)
                {
                    return NotFound($"Questão com Id: {id} não encontrada.");
                }
                return Ok(questionFromDb);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar a questão com ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar a questão.");
            }
        }

        //[Authorize]
        [HttpGet("question/session/{idSession}")]
        public async Task<IActionResult> GetQuestionIdSession(string idSession)
        {
            try
            {
                var questionsFromDb = await _questionApplication.GetQuestionByIdSession(idSession);
                if (questionsFromDb == null)
                {
                    return NotFound($"Questão com Id Session: {idSession} não encontrada.");
                }
                return Ok(questionsFromDb);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Você não tem permissão para acessar este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao buscar as questões para o ID da sessão: {idSession}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro ao buscar as questões.");
            }
        }
    }
}
