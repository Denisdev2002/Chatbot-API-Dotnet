using Application.Service.Interfaces;
using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service.Application
{
    public class SessionApplication : ISessionApplication
    {
        private readonly ISessionService _sessionService;

        public SessionApplication(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<List<Session>> GetSessions()
        {
            return await _sessionService.GetSessionsAsync();
        }

        public async Task InsertSession(SessionViewModel sessionViewModel)
        {
            await _sessionService.CreateSessionAsync(sessionViewModel);
        }

        public async Task UpdateSession(string session, SessionViewModel sessionViewModel)
        {
            await _sessionService.UpdateSessionAsync(session, sessionViewModel);
        }

        public async Task DeleteSessionId(string sessionId)
        {
            await _sessionService.DeleteSessionByIdAsync(sessionId);
        }

        public async Task<Session> GetSessionId(string sessionId)
        {
            return await _sessionService.GetSessionByIdAsync(sessionId);
        }
        public async Task<List<Session>> GetSessionUser(string email)
        {
            return await _sessionService.GetSessionByEmailAsync(email);
        }

        public async Task ActivateSession(string sessionId)
        {
            await _sessionService.ActivateSessionAsync(sessionId);
        }

        public async Task DeactivateSession(string sessionId)
        {
            await _sessionService.DeactivateSessionAsync(sessionId);
        }
    }

}