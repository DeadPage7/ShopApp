using ShopApp.Models;
using ShopApp.Services;
using ShopApp.Pages;

namespace ShopApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        // Обработка логина
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var login = LoginEntry.Text;
            var password = PasswordEntry.Text;

            var client = await _authService.LoginAsync(login, password);

            if (client != null)
            {
                // Успешный вход, создаем экземпляр сервиса для продуктов
                var productService = new ProductService();
                await Navigation.PushAsync(new HomePage(client, productService)); // Передаем данные клиента и сервис продуктов
            }
            else
            {
                // Ошибка авторизации
                await DisplayAlert("Ошибка", "Неверный логин или пароль", "OK");
            }
        }

        private async void OnRegisterPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage()); // Открываем страницу регистрации
        }
    }
}
