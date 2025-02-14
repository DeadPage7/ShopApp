using Microsoft.Maui.ApplicationModel.Communication;
using ShopApp.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Xml;

namespace ShopApp.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private Client _client;
    private string _token;

    // Конструктор страницы профиля, принимает объект клиента и токен
    public ProfilePage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;

        // Инициализация полей с данными клиента
        fullname.Text = client.FullName;
        email.Text = client.Email;
        login.Text = client.Login;
        birth.Text = client.Birth;
        phone.Text = string.IsNullOrEmpty(client.Telephone) ? "" : client.Telephone;
    }

    // Обработчик для кнопки сохранения данных
    private async void OnSaveButton(object sender, EventArgs e)
    {
        // Проверка на заполненность обязательных полей
        if (string.IsNullOrWhiteSpace(fullname.Text) ||
            string.IsNullOrWhiteSpace(email.Text) ||
            string.IsNullOrWhiteSpace(login.Text) ||
            string.IsNullOrWhiteSpace(birth.Text))
        {
            await DisplayAlert("Ошибка", "Не все обязательные поля заполнены!", "ОК");
            return;
        }

        // Проверка корректности формата даты рождения
        if (!DateTime.TryParse(birth.Text, out _))
        {
            await DisplayAlert("Ошибка", "Дата рождения должна быть в формате YYYY-MM-DD.", "ОК");
            return;
        }

        // Подготовка обновленных данных клиента
        var updatedUser = new Dictionary<string, object>
        {
            { "full_name", fullname.Text },
            { "email", email.Text },
            { "birth", birth.Text },
            { "telephone", string.IsNullOrEmpty(phone.Text) ? null : phone.Text }
        };

        // Если логин был изменен, добавляем его в данные для обновления
        if (login.Text != _client.Login)
        {
            updatedUser.Add("login", login.Text);
        }

        // Сериализация данных в формат JSON
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(updatedUser, options), Encoding.UTF8, "application/json");

        try
        {
            // Добавление токена авторизации в заголовки запроса
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            HttpResponseMessage response = await _httpClient.PutAsync($"http://course-project-4/api/profile", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Обновление данных клиента локально после успешного сохранения
                _client.FullName = fullname.Text;
                _client.Email = email.Text;
                _client.Birth = birth.Text;
                _client.Telephone = string.IsNullOrEmpty(phone.Text) ? null : phone.Text;

                if (login.Text != _client.Login)
                {
                    _client.Login = login.Text;
                }

                await DisplayAlert("Успех", "Данные сохранены!", "ОК");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Не удалось сохранить данные. {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Описание: {ex.Message}", "OK");
        }
    }

    // Переход на страницу заказов
    private async void OnOrdersButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrderPage(_client, _token));
    }

    // Переход на страницу адресов
    private async void OnAddressButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddressesPage(_client, _token));
    }

    // Переход на страницу каталога продуктов
    private async void OnCatalogButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductsPage(0, _client, _token)); // 0 для всех продуктов
    }

    // Переход на страницу профиля (обработчик кнопки Профиль)
    private async void OnProfileButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage(_client, _token));
    }

    // Переход на главную страницу
    private async void OnHomeButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage(_client, _token));
    }
}
