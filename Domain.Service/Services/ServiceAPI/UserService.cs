using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.Service.Interfaces;
using Domain.Service.Services.ServiceJwt;
using Domain.ViewModel;
using Infra.Interfaces;
using Infra.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;


namespace Domain.Service.Services.ServiceApi
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly HashPasswordService _hashPassword;
        private readonly RequestSessionService _requestSessionService;

        public UserService(
            ILogger<UserService> logger, 
            HashPasswordService hashPassword, 
            RequestSessionService requestSessionService
            )
        {
            _logger = logger;
            _hashPassword = hashPassword;
            _requestSessionService = requestSessionService;
        }

        public async Task<TokenDto> LoginAsync(LoginViewModel loginViewModel)
        {
            try
            {
                if (loginViewModel.login.IsNullOrEmpty())
                {
                    _logger.LogInformation($"Incorrect email : {loginViewModel.login}");

                }
                if (loginViewModel.senha.IsNullOrEmpty()) 
                {
                    _logger.LogInformation($"Incorrect password : {loginViewModel.senha}");
                    
                }
                Console.WriteLine($"Email : {loginViewModel.login}\nPassword : {loginViewModel.senha}");

                var user = await _requestSessionService.LoginIasApi(loginViewModel.login, loginViewModel.senha);
                if (user == null) throw new Exception("Usuário inexistente.");

                _logger.LogInformation($"User found: { user}");

                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário.");
                throw;
            }
        }
    }

}