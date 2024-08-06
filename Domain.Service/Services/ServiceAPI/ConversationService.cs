using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.ViewModel;
using Infra.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Service.Service.ServiceAPI
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<ConversationService> _logger;
        private readonly IQuestionRepository _questionRepository;

        public ConversationService(IConversationRepository conversationRepository,
            ILogger<ConversationService> logger,
            IQuestionRepository questionRepository)
        {
            _conversationRepository = conversationRepository;
            _logger = logger;
            _questionRepository = questionRepository;
        }
        public async Task<List<Conversation>> GetConversations()
        {
            try
            {
                return await _conversationRepository.GetAllConversationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conversas.");
                return new List<Conversation>();
            }
        }

        public async Task<Conversation> InsertConversation(ConversationViewModel conversationViewModel)
        {
            try
            {
                var question = await _questionRepository.GetQuestionByIdSessionAsync(conversationViewModel.IdSession);
                if (question == null)
                {
                    throw new Exception("Questão não encontrada.");
                }

                var conversation = new Conversation
                {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = conversationViewModel.SessionId,
                    IdSession = conversationViewModel.IdSession,
                    ResponseId = conversationViewModel.ResponseId

                };
                Console.WriteLine($"Conversa {conversation.Id} inserida.");
                return await _conversationRepository.AddConversationAsync(conversation.SessionId, conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir conversa.");
                throw;
            }
        }

        public async Task UpdateConversation(string id, ConversationViewModel conversationViewModel)
        {
            try
            {
                var originConversation = await _conversationRepository.GetConversationByIdAsync(id);
                if (originConversation == null)
                {
                    throw new Exception("Conversa não encontrada.");
                }

                originConversation.IdSession = conversationViewModel.IdSession;

                Console.WriteLine($"Id da sessão {id} atualizada. para {conversationViewModel.IdSession}.");
                await _conversationRepository.UpdateConversationAsync(originConversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar conversa.");
                throw;
            }
        }

        public async Task DeleteConversation(string id)
        {
            try
            {
                var originConversation = await _conversationRepository.GetConversationByIdAsync(id);
                if (originConversation == null)
                {
                    throw new Exception("Conversa não encontrada.");
                }

                await _conversationRepository.DeleteConversationAsync(originConversation);
                Console.WriteLine($"Conversa {id} deletada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar conversa.");
                throw;
            }
        }

        public async Task<Conversation> GetConversationById(string id)
        {
            try
            {
                return await _conversationRepository.GetConversationByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conversa.");
                return null;
            }
        }
    }
}