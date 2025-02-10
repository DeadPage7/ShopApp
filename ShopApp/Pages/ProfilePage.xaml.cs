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
    public ProfilePage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;
        fullname.Text = client.FullName;
        email.Text = client.Email;
        login.Text = client.Login;
        birth.Text = client.Birth;
        phone.Text = client.Telephone;
        if (string.IsNullOrEmpty(client.Telephone))
        {
            phone.Text = "";
        }
        else
        {
            phone.Text = client.Telephone;
        }
    }

    private async void OnSaveButton(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(fullname.Text) ||
            string.IsNullOrWhiteSpace(email.Text) ||
            string.IsNullOrWhiteSpace(login.Text) ||
            string.IsNullOrWhiteSpace(birth.Text))
        {
            await DisplayAlert("Ошибка", "Не все обязательные поля заполнены!", "ОК");
            return;
        }

        // Проверка корректности формата даты
        if (!DateTime.TryParse(birth.Text, out _))
        {
            await DisplayAlert("Ошибка", "Дата рождения должна быть в формате YYYY-MM-DD.", "ОК");
            return;
        }

        var updatedUser = new Dictionary<string, object>
    {
        { "full_name", fullname.Text },
        { "email", email.Text },
        { "birth", birth.Text },
        { "telephone", string.IsNullOrEmpty(phone.Text) ? null : phone.Text }

    };

        if (login.Text != _client.Login)
        {
            updatedUser.Add("login", login.Text);
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(updatedUser, options), Encoding.UTF8, "application/json");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            HttpResponseMessage response = await _httpClient.PutAsync($"http://course-project-4/api/profile", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Обновление клиента локально
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

    private async void OnOrdersButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrderPage(_client, _token));
    }

    private async void OnAddressButton(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddressesPage(_client, _token));
    }
    private async void OnCatalogButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductsPage(0, _client, _token)); // 0 для всех продуктов
    }

    // Обработчик кнопки Профиль
    private async void OnProfileButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage(_client, _token));
    }
    private async void OnHomeButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage(_client, _token));
    }
}