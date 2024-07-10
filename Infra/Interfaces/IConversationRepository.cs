using Domain.Entities;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConversationRepository
{
    Task<List<Conversation>> GetAllConversationsAsync();
    Task<Conversation> GetConversationByIdAsync(string id);
    Task<Conversation> AddConversationAsync(string id, Conversation conversation);
    Task UpdateConversationAsync(Conversation conversation);
    Task DeleteConversationAsync(Conversation conversationDTO);
}