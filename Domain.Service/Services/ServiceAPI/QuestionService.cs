using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.Service.Services.ServiceApiExternal;
using Domain.ViewModel;
using Infra.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.Service.Services.ServiceApi
{
    public class QuestionService : IQuestionService
    {
        private readonly RequestConversationService _requestConversation;
        private readonly IQuestionRepository _questionRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<QuestionService> _logger;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;

        public QuestionService(RequestConversationService requestConversation,
            IQuestionRepository questionRepository,
            IConversationRepository conversationRepository,
            ILogger<QuestionService> logger,
            ISessionRepository sessionRepository,
            IUserRepository userRepository)
        {
            _requestConversation = requestConversation;
            _questionRepository = questionRepository;
            _conversationRepository = conversationRepository;
            _logger = logger;
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
        }

        public async Task<Conversation> ToAsk(string idQuestion)
        {
            try
            {
                var question = await _questionRepository.GetQuestionByIdAsync(idQuestion);

                if (question == null)
                {
                    throw new Exception("Pergunta não encontrada.");
                }
                Console.WriteLine("Questão : " + question.text);
                var responseContent = await _requestConversation.MakePostRequest(question);

                var conversation = JsonConvert.DeserializeObject<Conversation>(responseContent);

                if (conversation == null)
                {
                    throw new Exception("Não foi possível deserializar a resposta.");
                }

                var insertConversation = new Conversation
                {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = conversation.SessionId,
                    IdSession = question.IdSession,
                    ResponseId = Guid.NewGuid().ToString(),
                    Response = new Response
                    {
                        ResponseId = conversation.ResponseId,
                        Result = conversation.Response.Result,
                        Sources = new List<Source>
                        {
                            new Source
                            {
                                SourceId = Guid.NewGuid().ToString(),
                                SourceDocument = conversation.Response.Sources[0].SourceDocument,
                                PageDocument = conversation.Response.Sources[0].PageDocument
                            }
                        }
                    }
                };

                Console.WriteLine("Session Id : " + conversation.SessionId);

                if (insertConversation.ResponseId == null)
                {
                    insertConversation.ResponseId = insertConversation.Response.ResponseId;
                }
                question.IdConversation = insertConversation.Id;

                await _conversationRepository.AddConversationAsync(insertConversation.IdSession, insertConversation);
                await _questionRepository.UpdateQuestionAsync(question);
                Console.WriteLine($"Conversa {conversation.Id} inserida.");
                return insertConversation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir conversa.");
                throw;
            }
        }

        public async Task<List<Question>> GetQuestions()
        {
            return await _questionRepository.GetQuestionsAsync();
        }

        public async Task<Question> GetQuestionById(string id)
        {
            return await _questionRepository.GetQuestionByIdAsync(id);
        }
        public async Task<Question> GetQuestionByIdSession(string id)
        {
            return await _questionRepository.GetQuestionByIdSessionAsync(id);
        }

        public async Task InsertQuestion(QuestionViewModel questionViewModel)
        {
            try
            {
                if (questionViewModel == null)
                {
                    throw new ArgumentNullException(nameof(questionViewModel), "Questão não pode ser nula.");
                }
                if (questionViewModel.IdSession == null)
                {
                    throw new ArgumentNullException(nameof(questionViewModel.IdSession), "Sessão não pode ser nula.");
                }
                var session = await _sessionRepository.GetSessionBySessionIdAsync(questionViewModel.IdSession);
                if (session == null)
                {
                    throw new Exception("Sessão não encontrada.");
                }
                var user = await _userRepository.GetUserByEmailAsync(session.EmailUser);
                if (user == null)
                {
                    throw new Exception("User não encontrado.");
                }
                var userType = user.Role == 0 ? "Admin" : "User";
                var question = new Question
                {
                    Id = Guid.NewGuid().ToString(),
                    IdSession = questionViewModel.IdSession,
                    text = questionViewModel.Text,
                    session_id = questionViewModel.Session_id, 
                    user_type = userType
                };

                await _questionRepository.InsertQuestionAsync(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conversa.");
                // Tratar objeto nulo
                throw;
            }
        }

        public async Task UpdateQuestion(string id, QuestionViewModel questionViewModel)
        {
            try
            {
                var originQuestion = await _questionRepository.GetQuestionByIdAsync(id);
                if (originQuestion == null)
                {
                    throw new Exception("Question não encontrada.");
                }
                originQuestion.text = questionViewModel.Text;
                Console.WriteLine($"Texto {originQuestion.text} atualizado para {questionViewModel.Text}.");
                await _questionRepository.UpdateQuestionAsync(originQuestion);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar conversa.");
                throw;
            }
        }

        public async Task DeleteQuestion(string id)
        {
            var originQuestion = await _questionRepository.GetQuestionByIdSessionAsync(id);
            if (originQuestion == null)
                throw new Exception("Question nao existe.");

            await _questionRepository.DeleteQuestionAsync(originQuestion);
        }


    }
}