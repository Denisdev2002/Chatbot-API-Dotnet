using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.Service.Services.ServiceApiExternal;
using Domain.ViewModel;
using Infra.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Service.Service.ServiceAPI
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<SessionService> _logger;
        private readonly RequestConversationService _requestConversationService;

        public SessionService(
            ISessionRepository sessionRepository,
            ILogger<SessionService> logger,
            RequestConversationService requestConversationService)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
            _requestConversationService = requestConversationService;
        }

        public async Task<List<Session>> GetSessionsAsync()
        {
            return await _sessionRepository.GetSessionsAsync();
        }

        public async Task<Session> CreateSessionAsync(SessionViewModel sessionViewModel)
        {
            if (sessionViewModel == null)
            {
                throw new ArgumentNullException(nameof(sessionViewModel), "A sessão não pode ser nula.");
            }
            Console.WriteLine("Sessão enviada : " + sessionViewModel);
            if (sessionViewModel.EmailUser.IsNullOrEmpty())
            {
                Console.WriteLine("Usuáio : "+ sessionViewModel.EmailUser);
                throw new ArgumentNullException(nameof(sessionViewModel.EmailUser), "Usuário não pode ser nula.");
            }

            var session = new Session
            {
                IdSession = Guid.NewGuid().ToString(),
                EmailUser = sessionViewModel.EmailUser

            };
            await _sessionRepository.InsertSessionAsync(session);
            return session;
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

        public async Task DeleteSessionIdExternal(string sessionId)
        {
            try
            {
                await _requestConversationService.DeleteSession(sessionId);
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
            //originSession.IdSession = sessionViewModel.Id;
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