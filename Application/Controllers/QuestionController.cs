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
        private readonly IDatabase _redis;

        public QuestionController(IQuestionApplication questionApplication, ILogger<QuestionController> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _questionApplication = questionApplication;
            _logger = logger;
            _redis = connectionMultiplexer.GetDatabase();
        }

        //[Authorize]
        [HttpGet("toask/{idQuestion}")]
        public async Task<IActionResult> ToAskForModel(string idQuestion)
        {
            try
            {
                var cacheKey = $"question:ask:{idQuestion}";

                var cachedResponse = await _redis.StringGetAsync(cacheKey);

                if (!cachedResponse.IsNullOrEmpty)
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(cachedResponse);
                    return Ok(response);
                }

                var responseFromModel = await _questionApplication.ToAsk(idQuestion);

                var responseJson = JsonConvert.SerializeObject(responseFromModel);
                await _redis.StringSetAsync(cacheKey, responseJson, TimeSpan.FromMinutes(10));

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
                // Check cache first
                var cachedQuestions = await _redis.StringGetAsync("allQuestions");
                if (!cachedQuestions.IsNullOrEmpty)
                {
                    var questions = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<QuestionViewModel>>(cachedQuestions);
                    return Ok(questions);
                }
                var questionsFromDb = await _questionApplication.GetQuestions();
                var questionsJson = System.Text.Json.JsonSerializer.Serialize(questionsFromDb);
                await _redis.StringSetAsync("allQuestions", questionsJson, TimeSpan.FromMinutes(10));

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

                // Invalidate cache
                await _redis.KeyDeleteAsync("allQuestions");

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
                await _questionApplication.UpdateQuestion(id, questionViewModel);

                // Invalidate cache
                await _redis.KeyDeleteAsync("allQuestions");

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

                // Invalidate cache
                await _redis.KeyDeleteAsync("allQuestions");

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
                var cachedQuestion = await _redis.StringGetAsync($"question:{id}");
                if (!cachedQuestion.IsNullOrEmpty)
                {
                    var question = System.Text.Json.JsonSerializer.Deserialize<QuestionViewModel>(cachedQuestion);
                    return Ok(question);
                }

                var questionFromDb = await _questionApplication.GetQuestionById(id);
                if (questionFromDb == null)
                {
                    return NotFound($"Questão com Id: {id} não encontrada.");
                }

                var questionJson = System.Text.Json.JsonSerializer.Serialize(questionFromDb);
                await _redis.StringSetAsync($"question:{id}", questionJson, TimeSpan.FromMinutes(10));

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
                var cachedQuestions = await _redis.StringGetAsync($"session:{idSession}");
                if (!cachedQuestions.IsNullOrEmpty)
                {
                    var questions = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<QuestionViewModel>>(cachedQuestions);
                    return Ok(questions);
                }

                var questionsFromDb = await _questionApplication.GetQuestionByIdSession(idSession);
                if (questionsFromDb == null)
                {
                    return NotFound($"Questão com Id Session: {idSession} não encontrada.");
                }

                var questionsJson = System.Text.Json.JsonSerializer.Serialize(questionsFromDb);
                await _redis.StringSetAsync($"session:{idSession}", questionsJson, TimeSpan.FromMinutes(10));

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
