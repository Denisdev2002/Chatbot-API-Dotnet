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