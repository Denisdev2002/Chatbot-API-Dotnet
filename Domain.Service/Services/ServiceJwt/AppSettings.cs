
namespace Domain.Service.Services.ServiceJwt
{
    public class AppSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int TokenTime { get; set; } = 3;
    }
}