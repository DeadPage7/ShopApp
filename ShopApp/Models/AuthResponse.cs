using System.Text.Json.Serialization;

namespace ShopApp.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("client")] // Маппинг для объекта клиента
        public Client Client { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; } // Токен для авторизации
    }
}
