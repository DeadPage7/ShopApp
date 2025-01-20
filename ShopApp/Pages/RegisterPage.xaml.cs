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
        private readonly HttpClient _httpClient = new HttpClient();
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Ïðîâåðêà íà ïóñòûå ïîëÿ
            if (string.IsNullOrWhiteSpace(RegisterLoginEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterPasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterPasswordConfirmationEntry.Text) ||
                string.IsNullOrWhiteSpace(RegisterFullNameEntry.Text) ||
                    string.IsNullOrWhiteSpace(RegisterEmailEntry.Text) ||
                    string.IsNullOrWhiteSpace(RegisterBirthEntry.Text))
            {
                await DisplayAlert("Ошибка", "Все обязательные поля должны быть заполнены", "ОК");
                return;
            }

            if (RegisterPasswordEntry.Text != RegisterPasswordConfirmationEntry.Text)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
                return;
            }
            string fullname = RegisterFullNameEntry.Text;
            string login = RegisterLoginEntry.Text;
            string password = RegisterPasswordEntry.Text;
            string? phone = RegisterTelephoneEntry.Text;

            var registerData = new MultipartFormDataContent
        {
            { new StringContent(fullname), "full_name" },
            { new StringContent(phone ?? string.Empty), "telephone" },
            { new StringContent(login), "login" },
            { new StringContent(password), "password" },
        };

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/register", registerData);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успех", "Зарегистрированы", "ОК");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", $"Описание: {(int)response.StatusCode}\n{errorContent}", "ОК");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "ОК");
            }
        }

        // Переход на страницу авторизации
        private async void OnLoginPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage()); // Переход на страницу логина
        }
    }
}
