using Newtonsoft.Json;

namespace Domain.Entities.DataTransferObject
{
    public class TokenDto
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }
    }
}
