using Domain.Entities;
using Domain.ViewModel;
using Microsoft.VisualBasic;

namespace Application.Service.Interfaces
{
    public interface IConversationApplication
    {
        Task<List<Conversation>> GetAllConversations();
        Task CreateConversation(ConversationViewModel conversationViewModel);
        Task UpdateConversation(string id, ConversationViewModel conversationViewModel);
        Task DeleteConversation(string conversationId);
        Task<Conversation> GetByIdConversation(string conversationId);
    }
}