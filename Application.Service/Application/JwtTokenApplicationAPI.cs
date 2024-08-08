using Application.Service.Interface;
using Domain.Service.Interfaces;

namespace Application.Service.Application
{
    public class JwtTokenApplicationAPI : IJwtTokenApplicationAPI
    {
        private readonly IJwtTokenServiceAPI _jwtTokenServiceAPI;
        public JwtTokenApplicationAPI(IJwtTokenServiceAPI jwtTokenServiceAPI)
        {
            _jwtTokenServiceAPI = jwtTokenServiceAPI;
        }
        public Task<string> GetKeySecretToken()
        {
            return _jwtTokenServiceAPI.GetKeySecretToken();
        }
    }
}
