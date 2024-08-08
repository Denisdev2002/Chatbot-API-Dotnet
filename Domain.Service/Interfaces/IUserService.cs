using Domain.Entities;
using Domain.Entities.DataTransferObject;
using Domain.ViewModel;

namespace Domain.Service.Interfaces
{
    public interface IUserService
    {
        Task<TokenDto> LoginAsync(LoginViewModel loginViewModel);
    }
}