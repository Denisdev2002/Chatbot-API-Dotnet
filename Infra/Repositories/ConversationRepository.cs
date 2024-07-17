using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Infra.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ContextDatabase _context;
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<ConversationRepository> _logger;

        public ConversationRepository(
            ContextDatabase context, 
            ILogger<ConversationRepository> logger, 
            ISessionRepository sessionrepository
            )
        {
            _context = context;
            _logger = logger;
            _sessionRepository = sessionrepository;
        }

        public async Task<List<Conversation>> GetAllConversationsAsync()
        {
            return await _context.Conversations
                .Include(c => c.Response)
                    .ThenInclude(r => r.Sources)
                .ToListAsync();
        }

        public async Task<Conversation> GetConversationByIdAsync(string id)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Response)
                    .ThenInclude(r => r.Sources)
                    .FirstOrDefaultAsync(c => c.Id == id);
            if (conversation == null)
            {
                throw new ArgumentNullException(nameof(conversation), "Conversa não encontrada.");

            }
            Console.WriteLine("Conversa : " + conversation.Id);
            return conversation;
        }

        public async Task<Conversation> AddConversationAsync(string id, Conversation conversation)
        {
            var idSession = _sessionRepository.GetSessionBySessionIdAsync(id);

            if (idSession == null)
            {
                throw new Exception("Sessão não encontrada.");
            }

            try
            {
                Console.WriteLine("Id passado : " + id);
                Console.WriteLine("Sessão : " + idSession.Result.IdSession);
                //Console.WriteLine("Id da sessão : " + idSession.Id);
                if (conversation.IdSession != idSession.Result.IdSession)
                {
                    throw new Exception("Questão não pertence a essa sessão");
                }
                var result = await _context.Conversations.AddAsync(conversation);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir conversa.");
                throw;
            }
        }

        public async Task UpdateConversationAsync(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConversationAsync(Conversation conversation)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
        }
    }
}