using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.ViewModel;

namespace Domain.Service.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<TokenDto> LoginAsync(LoginViewModel loginViewModel);
    }
}