using Newtonsoft.Json;

namespace ShopApp.Models
{
    public class Client
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("birth")]
        public DateTime Birth { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; } // Токен для авторизации
    }
    public class AuthResponse
    {
        [JsonProperty("client")] // Маппинг для объекта клиента
        public Client Client { get; set; }

        [JsonProperty("message")] // Если есть сообщение в ответе
        public string Message { get; set; }
    }
}
