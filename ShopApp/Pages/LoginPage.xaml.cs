using ShopApp.Models;
using ShopApp.Pages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ShopApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient(); // Создаём экземпляр HttpClient для работы с API

        // Конструктор страницы входа
        public LoginPage()
        {
            InitializeComponent();
        }

        // Обработчик события при клике на кнопку входа
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string login = LoginEntry.Text; // Получаем текст из поля ввода логина
            string password = PasswordEntry.Text; // Получаем текст из поля ввода пароля

            // Проверка, чтобы логин и пароль не были пустыми
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите логин и пароль", "ОК"); // Если не введены, показываем ошибку
                return;
            }

            try
            {
                // Пытаемся аутентифицировать пользователя
                var loginResponse = await AuthenticateUserAsync(login, password);

                if (loginResponse != null)
                {
                    // Если аутентификация прошла успешно, переходим на главную страницу
                    await Navigation.PushAsync(new HomePage(loginResponse.Client, loginResponse.Token));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК"); // Если произошла ошибка, показываем её
            }
        }

        // Асинхронный метод для аутентификации пользователя
        private async Task<AuthResponse> AuthenticateUserAsync(string login, string password)
        {
            var loginData = new { login, password }; // Создаём объект с данными для входа
            var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json"); // Преобразуем данные в JSON

            // Отправляем POST-запрос на сервер
            HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/login", jsonContent);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync(); // Читаем ответ от сервера
                return JsonSerializer.Deserialize<AuthResponse>(content); // Десериализуем ответ в объект AuthResponse
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await DisplayAlert("Ошибка входа", "Неправильный логин или пароль", "ОК"); // Если неправильный логин или пароль
            }
            else
            {
                await DisplayAlert("Ошибка", "Произошла ошибка на сервере", "ОК"); // Если ошибка сервера
            }

            return null;
        }

        // Обработчик события при клике на ссылку регистрации
        private async void OnRegisterPageClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage()); // Переход на страницу регистрации
        }
    }
}
