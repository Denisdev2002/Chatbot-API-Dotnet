using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.ViewModel;
using Infra.Interfaces;
using Infra.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.Service.Service.ServiceAPI
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<SessionService> _logger;
        private readonly IUserRepository _userRepository;

        public SessionService(
            ISessionRepository sessionRepository,
            ILogger<SessionService> logger,
            IUserRepository userRepository)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
            _userRepository = userRepository;

        }

        public async Task<List<Session>> GetSessionsAsync()
        {
            return await _sessionRepository.GetSessionsAsync();
        }

        public async Task CreateSessionAsync(SessionViewModel sessionViewModel)
        {
            if (sessionViewModel == null)
            {
                throw new ArgumentNullException(nameof(sessionViewModel), "A sessão não pode ser nula.");
            }
            if (sessionViewModel.EmailUser == null)
            {
                throw new ArgumentNullException(nameof(sessionViewModel.EmailUser), "Usuário não pode ser nula.");
            }

            var user = _userRepository.GetUserByEmailAsync(sessionViewModel.EmailUser);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User não pode ser nulo.");
            }

            var session = new Session
            {
                IdSession = Guid.NewGuid().ToString(),
                EmailUser = user.Result.Email,
                IsActive = true
            };
            await _sessionRepository.InsertSessionAsync(session);
        }

        public async Task DeleteSessionByIdAsync(string sessionId)
        {
            try
            {
                var session = await _sessionRepository.GetSessionBySessionIdAsync(sessionId);
                if (session == null)
                {
                    throw new Exception("Sessão não encontrada.");
                }

                await _sessionRepository.DeleteSessionAsync(session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public async Task UpdateSessionAsync(string id, SessionViewModel sessionViewModel)
        {
            var originSession = await _sessionRepository.GetSessionBySessionIdAsync(id);
            if (originSession == null)
                throw new Exception("Sessão não existe");
            originSession.IdSession = sessionViewModel.Id;
            originSession.EmailUser = sessionViewModel.EmailUser;

            await _sessionRepository.UpdateSessionAsync(originSession);
        }

        public async Task<Session> GetSessionByIdAsync(string id)
        {
            var session = await _sessionRepository.GetSessionBySessionIdAsync(id);
            return session;
        }
        public async Task<List<Session>> GetSessionByEmailAsync(string id)
        {
            var session = await _sessionRepository.GetSessionUserEmailAsync(id);
            return session;
        }

        public async Task ActivateSessionAsync(string idSession)
        {
            await _sessionRepository.ActivateSessionAsync(idSession);
        }

        public async Task DeactivateSessionAsync(string idSession)
        {
            await _sessionRepository.DeactivateSessionAsync(idSession);
        }
    }

}