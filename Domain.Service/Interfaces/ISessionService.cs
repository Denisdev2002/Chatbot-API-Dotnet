using Domain.Entities;
using Domain.ViewModel;

namespace Domain.Service.Interfaces
{
    public interface ISessionService
    {
        Task<List<Session>> GetSessionsAsync();
        Task<Session> CreateSessionAsync(SessionViewModel sessionViewModel);
        Task UpdateSessionAsync(string id, SessionViewModel sessioViewModel);
        Task DeleteSessionByIdAsync(string sessionId);
        Task<Session> GetSessionByIdAsync(string id);
        Task<List<Session>> GetSessionByEmailAsync(string id);
        Task ActivateSessionAsync(string sessionId);
        Task DeactivateSessionAsync(string sessionId);
    }

}