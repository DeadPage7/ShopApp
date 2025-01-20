using ShopApp.Models;
using ShopApp.Pages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ShopApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string login = LoginEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите логин и пароль", "ОК");
                return;
            }

            try
            {
                var loginResponse = await AuthenticateUserAsync(login, password);

                if (loginResponse != null)
                {

                    // Переход на главную страницу
                    await Navigation.PushAsync(new HomePage(loginResponse.Client, loginResponse.Token));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК");
            }
            finally
            {

            }
        }

        private async Task<AuthResponse> AuthenticateUserAsync(string login, string password)
        {
            var loginData = new { login, password };
            var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/login", jsonContent);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthResponse>(content);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await DisplayAlert("Ошибка входа", "Неправильный логин или пароль", "ОК");
            }
            else
            {
                await DisplayAlert("Ошибка", "Произошла ошибка на сервере", "ОК");
            }

            return null;
        }
private async void OnRegisterPageClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
    
}
