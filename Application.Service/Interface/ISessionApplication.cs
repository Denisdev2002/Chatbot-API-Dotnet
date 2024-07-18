using Domain.Entities;
using Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service.Interfaces
{
    public interface ISessionApplication
    {
        Task<List<Session>> GetSessions();
        Task<Session> InsertSession(SessionViewModel sessionViewModel);
        Task UpdateSession(string session, SessionViewModel sessionViewModel);
        Task DeleteSessionId(string sessionId);
        Task<Session> GetSessionId(string sessionId);
        Task<List<Session>> GetSessionUser(string id);
        Task ActivateSession(string sessionId);
        Task DeactivateSession(string sessionId);
    }

}