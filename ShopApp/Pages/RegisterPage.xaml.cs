using ShopApp.Models;
using ShopApp.Services;
using System;

namespace ShopApp.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegisterPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        // Метод для регистрации
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Получаем данные из полей
            var login = RegisterLoginEntry.Text;
            var password = RegisterPasswordEntry.Text;
            var passwordConfirmation = RegisterPasswordConfirmationEntry.Text;
            var fullName = RegisterFullNameEntry.Text;
            var email = RegisterEmailEntry.Text;
            var birth = RegisterBirthEntry.Date;
            var telephone = RegisterTelephoneEntry.Text;

            // Выполняем запрос на регистрацию
            var client = await _authService.RegisterAsync(login, password, passwordConfirmation, fullName, email, birth, telephone);

            // Проверяем, что регистрация прошла успешно
            if (client != null)
            {
                // Переход на главную страницу и передача данных клиента и сервиса для продуктов
                var productService = new ProductService();
                await Navigation.PushAsync(new HomePage(client, productService)); // Передаем объект Client и сервис для продуктов
            }
            else
            {
                // Показать сообщение об ошибке, если регистрация не удалась
                await DisplayAlert("Ошибка", "Не удалось зарегистрировать пользователя. Попробуйте снова.", "OK");
            }
        }

        // Переход на страницу авторизации
        private async void OnLoginPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage()); // Переход на страницу логина
        }
    }
}
