using Newtonsoft.Json;
using ShopApp.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://course-project-4/api/login"; // URL вашего API

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        // Метод для логина
        public async Task<Client> LoginAsync(string login, string password)
        {
            var requestData = new
            {
                login = login,
                password = password
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Десериализация в объект AuthResponse
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseData);

                if (authResponse != null && authResponse.Client != null)
                {
                    // Логируем для проверки
                    Console.WriteLine($"Получены данные клиента: {authResponse.Client.FullName}, {authResponse.Client.Email}");
                    return authResponse.Client; // Возвращаем объект Client с данными
                }
                else
                {
                    Console.WriteLine("Ошибка: не удалось распарсить данные клиента.");
                }
            }

            // Логируем ошибку, если не удалось авторизовать
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ошибка авторизации: {errorMessage}");

            return null; // В случае ошибки авторизации
        }

        // Метод для регистрации
        public async Task<Client> RegisterAsync(string login, string password, string passwordConfirmation, string fullName, string email, DateTime birth, string telephone)
        {
            var requestData = new
            {
                login = login,
                password = password,
                password_confirmation = passwordConfirmation,
                full_name = fullName,
                email = email,
                birth = birth.ToString("yyyy-MM-dd"),
                telephone = telephone
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://course-project-4/api/register", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Десериализация ответа в AuthResponse
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseData);

                if (authResponse != null && authResponse.Client != null)
                {
                    // Логируем для проверки
                    Console.WriteLine($"Регистрация прошла успешно: {authResponse.Client.FullName}, {authResponse.Client.Email}");
                    return authResponse.Client; // Возвращаем объект Client с данными
                }
                else
                {
                    Console.WriteLine("Ошибка: не удалось распарсить данные клиента.");
                }
            }

            // Логируем ошибку, если регистрация не удалась
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ошибка регистрации: {errorMessage}");

            return null; // В случае ошибки регистрации
        }
    }
}
