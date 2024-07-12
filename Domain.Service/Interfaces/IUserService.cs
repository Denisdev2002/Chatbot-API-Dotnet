using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.ViewModel;

namespace Domain.Service.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
        Task InsertUserAsync(UserViewModel userViewModel);
        Task UpdateUserAsync(string email, UserViewModel userViewModel);
        Task DeleteUserAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<TokenDto> LoginAsync(LoginViewModel loginViewModel);
    }
}