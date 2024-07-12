using Application.Service.Interfaces;
using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.Service.Interfaces;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service.Application
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserService _userService;

        public UserApplication(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userService.GetUsersAsync();
        }

        public async Task InsertUserAsync([FromBody] UserViewModel userViewModel)
        {
            await _userService.InsertUserAsync(userViewModel);
        }


        public async Task UpdateUserAsync(string email, UserViewModel userViewModel)
        {
            await _userService.UpdateUserAsync(email, userViewModel);
        }

        public async Task DeleteUserAsync(string email)
        {
            await _userService.DeleteUserAsync(email);
        }

        public async Task<TokenDto> LoginAsync(LoginViewModel loginViewModel)
        {
            return await _userService.LoginAsync(loginViewModel);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userService.GetUserByEmailAsync(email);
        }
    }
}