using Microsoft.Extensions.Logging.Abstractions;
using ShopApp.Models;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace ShopApp.Pages
{
    public partial class RegisterPage : ContentPage
    {
        // Инициализация HTTP-клиента для отправки запросов
        private readonly HttpClient _httpClient = new HttpClient();

        public RegisterPage()
        {
            InitializeComponent();
        }

        // Обработчик нажатия кнопки регистрации
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Проверка на пустоту обязательных полей
            if (string.IsNullOrWhiteSpace(RegisterLoginEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterPasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterPasswordConfirmationEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterFullNameEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterEmailEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterBirthEntry.Text))
            {
                // Показать предупреждение о незаполненных полях
                await DisplayAlert("Ошибка", "Все обязательные поля должны быть заполнены", "ОК");
                return;
            }

            // Проверка на совпадение паролей
            if (RegisterPasswordEntry.Text != RegisterPasswordConfirmationEntry.Text)
            {
                // Показать ошибку, если пароли не совпадают
                await DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
                return;
            }

            // Сбор данных для отправки на сервер
            string fullname = RegisterFullNameEntry.Text;
            string login = RegisterLoginEntry.Text;
            string password = RegisterPasswordEntry.Text;
            string? phone = RegisterTelephoneEntry.Text;

            // Формируем тело запроса с данными
            var registerData = new MultipartFormDataContent
            {
                { new StringContent(fullname), "full_name" },
                { new StringContent(phone ?? string.Empty), "telephone" },
                { new StringContent(login), "login" },
                { new StringContent(password), "password" },
            };

            try
            {
                // Отправляем POST-запрос на сервер
                HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/register", registerData);

                // Проверка успешности запроса
                if (response.IsSuccessStatusCode)
                {
                    // Показать сообщение об успешной регистрации
                    await DisplayAlert("Успех", "Зарегистрированы", "ОК");
                }
                else
                {
                    // Если ошибка, показываем подробности
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", $"Описание: {(int)response.StatusCode}\n{errorContent}", "ОК");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при запросах
                await DisplayAlert("Ошибка", ex.Message, "ОК");
            }
        }

        // Обработчик для перехода на страницу авторизации
        private async void OnLoginPageClicked(object sender, EventArgs e)
        {
            // Переход на страницу логина
            await Navigation.PushAsync(new LoginPage());
        }
    }
}
