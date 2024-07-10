using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ContextDatabase _contextDatabase;


        public SessionRepository(ContextDatabase contextDatabase)
        {
            _contextDatabase = contextDatabase;
        }

        public async Task<List<Session>> GetSessionUserEmailAsync(string email)
        {
            var sessions = await _contextDatabase.Sessions
                .Include(s => s.Question)
                .Include(s => s.Conversations)
                    .ThenInclude(c => c.Response)
                        .ThenInclude(r => r.Sources)
                    .Where(s => s.EmailUser == email && s.IsActive == true).ToListAsync();
            Console.WriteLine("Quantidade : " + sessions.Count);
            if (sessions == null || sessions.Count == 0)
            {
                throw new ArgumentNullException(nameof(sessions), "Esse usuário não possui nenhuma sessão.");

            }
            Console.WriteLine("Id session repository : " + sessions[0].IdSession);
            return sessions;
        }
        public async Task<Session> GetSessionBySessionIdAsync(string id)
        {
            var session = await _contextDatabase.Sessions
                .Include(s => s.Question)
                .Include(s => s.Conversations)
                    .ThenInclude(c => c.Response)
                        .ThenInclude(r => r.Sources).FirstOrDefaultAsync(s => s.IdSession == id);
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session), "Sessão não encontrada.");

            }
            Console.WriteLine("Id session repository : " + session.IdSession);
            return session;
        }

        public async Task InsertSessionAsync(Session session)
        {
            _contextDatabase.Sessions.Add(session);
            await _contextDatabase.SaveChangesAsync();
        }

        public async Task UpdateSessionAsync(Session session)
        {
            _contextDatabase.Sessions.Update(session);

            try
            {
                await _contextDatabase.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                throw new Exception("Erro ao atualizar a sessão.", ex);
            }
        }

        public async Task DeleteSessionAsync(Session session)
        {
            _contextDatabase.Sessions.Remove(session);
            await _contextDatabase.SaveChangesAsync();
        }

        public async Task<List<Session>> GetSessionsAsync()
        {
            return await _contextDatabase.Sessions
                .Include(s => s.Question)
                .Include(s => s.Conversations)
                    .ThenInclude(c => c.Response)
                        .ThenInclude(r => r.Sources)
                .ToListAsync();
        }


        public async Task ActivateSessionAsync(string sessionId)
        {
            var session = await GetSessionBySessionIdAsync(sessionId);
            if (session != null)
            {
                session.IsActive = true;
                await UpdateSessionAsync(session);
            }
        }

        public async Task DeactivateSessionAsync(string sessionId)
        {
            var session = await GetSessionBySessionIdAsync(sessionId);
            if (session != null)
            {
                session.IsActive = false;
                await UpdateSessionAsync(session);
            }
        }
    }
}