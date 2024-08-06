using Application.Service.Interfaces;
using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.Service.Services.ServiceApi;
using Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service.Application
{
    public class ConversationApplication : IConversationApplication
    {
        private readonly IConversationService _conversationService;

        public ConversationApplication(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<Conversation> CreateConversation(ConversationViewModel conversationViewModel)
        {
            return await _conversationService.InsertConversation(conversationViewModel);
        }

        public async Task DeleteConversation(string conversationId)
        {
            await _conversationService.DeleteConversation(conversationId);
        }

        public async Task<List<Conversation>> GetAllConversations()
        {
            return await _conversationService.GetConversations();
        }

        public async Task<Conversation> GetByIdConversation(string conversationId)
        {
            return await _conversationService.GetConversationById(conversationId);
        }

        public async Task UpdateConversation(string id, ConversationViewModel conversationViewModel)
        {
            await _conversationService.UpdateConversation(id, conversationViewModel);
        }
    }
}