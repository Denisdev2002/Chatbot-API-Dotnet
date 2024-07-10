using Domain.Entities;

public interface ISessionRepository
{
    Task<Session> GetSessionBySessionIdAsync(string sessionId);
    Task<List<Session>> GetSessionUserEmailAsync(string email);
    Task InsertSessionAsync(Session session);
    Task UpdateSessionAsync(Session session);
    Task DeleteSessionAsync(Session session);
    Task<List<Session>> GetSessionsAsync();
    Task ActivateSessionAsync(string sessionId);
    Task DeactivateSessionAsync(string sessionId);
}