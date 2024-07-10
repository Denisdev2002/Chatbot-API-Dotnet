using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.Service.Interfaces;
using Domain.Service.Services.ServiceJwt;
using Domain.ViewModel;
using Infra.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Service.Services.ServiceApi
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly HashPasswordService _hashPassword;
        private readonly JwtTokenService _jwtTokenService;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, HashPasswordService hashPassword, JwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _hashPassword = hashPassword;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(user => new User
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            }).ToList();
        }

        public async Task InsertUserAsync(UserViewModel userViewModel)
        {
            var user = new User
            {
                UserName = userViewModel.Name,
                Email = userViewModel.Email,
                NormalizedEmail = userViewModel.Email,
                PhoneNumber = userViewModel.Number,
                Role = userViewModel.Role,
                PasswordHash = _hashPassword.Password(userViewModel.Password)
            };
            await _userRepository.InsertUserAsync(user);
        }


        public async Task UpdateUserAsync(string email, User user)
        {
            var originUser = await _userRepository.GetUserByEmailAsync(email);
            if (originUser == null)
                throw new Exception("Usuário não encontrado.");

            // Hash da senha antes de atualizar
            user.PasswordHash = _hashPassword.Password(user.PasswordHash);

            originUser.UserName = user.UserName;
            originUser.Email = user.Email;
            originUser.PasswordHash = user.PasswordHash;

            await _userRepository.UpdateUserAsync(originUser);
        }


        public async Task DeleteUserAsync(string email)
        {
            var originUser = await _userRepository.GetUserByEmailAsync(email);
            if (originUser == null)
                throw new Exception("Usuário não encontrado.");

            await _userRepository.DeleteUserAsync(originUser);
        }


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<TokenDto> LoginAsync(LoginViewModel loginViewModel)
        {
            try
            {
                _logger.LogInformation("Login attempt with email: {email}", loginViewModel.Email);

                var user = await _userRepository.GetUserByEmailAsync(loginViewModel.Email);
                if (user == null) throw new Exception("Usuário inexistente.");

                _logger.LogInformation("User found: {userId}", user.Id);

                if (!_hashPassword.VerifyPassword(loginViewModel.Password, user.PasswordHash))
                    throw new InvalidOperationException("Senha incorreta.");

                _logger.LogInformation("Password verified successfully for user: {userId}", user.Id);

                var token = _jwtTokenService.CreateToken(user);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário.");
                throw;
            }
        }
    }

}