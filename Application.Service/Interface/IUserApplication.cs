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
        Task<TokenDto> LoginAsync(LoginViewModel loginViewModel);
    }
}