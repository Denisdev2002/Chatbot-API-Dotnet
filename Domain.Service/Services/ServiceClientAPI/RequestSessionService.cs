using Domain.Entities.DataTransferObject;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Infra.Repositories
{
    public class RequestSessionService
    {
        private readonly ILogger<RequestSessionService> _logger;

        public RequestSessionService(ILogger<RequestSessionService> logger)
        {
            _logger = logger;
        }

        public async Task<TokenDto> LoginIasApi(string email, string password)
        {
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://192.168.10.40:92")
                };

                var loginData = new
                {
                    login = email,
                    senha = password
                };

                var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                var result = await client.PostAsync("/api/token/login", content);

                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"API call failed with status code {result.StatusCode}");
                    throw new Exception("API call failed");
                }

                string resultContent = await result.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(resultContent))
                {
                    _logger.LogError("Result content is null or empty");
                    throw new Exception("Result content is null or empty");
                }

                var serializeResult = JsonConvert.DeserializeObject<TokenDto>(resultContent);
                if (serializeResult == null)
                {
                    _logger.LogError("Deserialization result is null");
                    throw new Exception("Deserialization result is null");
                }

                _logger.LogInformation($"Login successful for user: {email}");
                Console.WriteLine(serializeResult.AccessToken);
                return serializeResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while logging in user: {email}");
                throw;
            }
        }

    }
}
