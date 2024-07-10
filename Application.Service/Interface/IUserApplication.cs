using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service.Interfaces
{
    public interface IUserApplication
    {
        Task<List<User>> GetUsersAsync();
        Task InsertUserAsync([FromBody] UserViewModel userViewModel);
        Task UpdateUserAsync(string email, User user);
        Task DeleteUserAsync(string email);
        Task<TokenDto> LoginAsync(LoginViewModel loginViewModel);
        Task<User?> GetUserByEmailAsync(string email);
    }
}