using Domain.Entities.DataTransferObject;
using Domain.Service.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Domain.Service.Services.ServiceJwt
{
    public class JwtTokenServiceAPI : IJwtTokenServiceAPI
    {
        private readonly HttpClient _client;
        private readonly ILogger<JwtTokenServiceAPI> _logger;
        private readonly ModelRuntimeInitializerDependencies _initializeTask;

        public Task InitializeAsync()
        {
            return _initializeTask.Initialize;
        }

        public JwtTokenServiceAPI(
            HttpClient client,
            ILogger<JwtTokenServiceAPI> logger
        )
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> GetKeySecretToken()
        {
            try
            {
                var resource1 = new Resources
                {
                    nome = "Recurso",
                    acao = "Criar"
                };
                var resource2 = new Resources
                {
                    nome = "Recurso",
                    acao = "Visualizar"
                };

                var resources = new List<Resources> { resource1, resource2 };

                var requestBody = new
                {
                    appToken = "50a069cc-155e-41b1-899d-d7aca36a3d0b",
                    recursos = resources
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var result = await _client.PutAsync("/api/registrarapp", content);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"API call failed with status code {result.StatusCode} and response: {errorContent}");
                    throw new Exception("API call failed");
                }

                string resultContent = await result.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(resultContent))
                {
                    _logger.LogError("Result content is null or empty");
                    throw new Exception("Result content is null or empty");
                }

                var response = JsonConvert.DeserializeObject<ApiResponse>(resultContent);
                if (response == null)
                {
                    _logger.LogError("Deserialization result is null");
                    throw new Exception("Deserialization result is null");
                }

                _logger.LogInformation($"App Token: {response.AppToken}");
                foreach (var res in response.Recursos)
                {
                    _logger.LogInformation($"Resource: {res.nome}, Action: {res.acao}");
                }
                return response.AppToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the key secret token.");
                throw;
            }
        }
    }

}
