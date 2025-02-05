using ShopApp.Models;
using System.Text.Json;
using System.Text;

namespace ShopApp.Pages;

public partial class CreateAddressPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private Client _client;
    private readonly string _token;

    public CreateAddressPage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;
    }

    private async void OnSaveAddressClicked(object sender, EventArgs e)
    {
        var newAddress = new Address
        {
            City = CityEntry.Text,
            Street = StreetEntry.Text,
            House = HouseEntry.Text,
            Floor = int.TryParse(FloorEntry.Text, out int floor) ? floor : null,
            ApartmentOrOffice = ApartmentOrOfficeEntry.Text,
            Entrance = EntranceEntry.Text,
            Intercom = IntercomEntry.Text,
            Comment = CommentEditor.Text
        };

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var jsonContent = JsonSerializer.Serialize(newAddress);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://course-project-4/api/addresses", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успех", "Адрес успешно добавлен!", "ОК");
                await Navigation.PopAsync(); // Возвращаемся на предыдущую страницу
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Код: {response.StatusCode}\n{errorContent}", "ОК");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК");
        }
    }
}
