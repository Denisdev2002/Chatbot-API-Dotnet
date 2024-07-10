using Domain.Entities;
using Domain.ViewModel;

namespace Domain.Service.Interfaces
{
    public interface IConversationService
    {
        Task<List<Conversation>> GetConversations();
        Task InsertConversation(ConversationViewModel conversationViewModel);
        Task UpdateConversation(string id, ConversationViewModel conversationViewModel);
        Task DeleteConversation(string id);
        Task<Conversation> GetConversationById(string id);
    }
}