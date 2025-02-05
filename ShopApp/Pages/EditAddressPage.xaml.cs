using ShopApp.Models;
using System.Text;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class EditAddressPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private Client _client;
    private Address _address;
    private readonly string _token;

    public EditAddressPage(Address address, Client client, string token)
    {
        InitializeComponent();
        _address = address;
        _client = client;
        _token = token;

        CityEntry.Text = _address.City;
        StreetEntry.Text = _address.Street;
        HouseEntry.Text = _address.House;
        FloorEntry.Text = _address.Floor?.ToString();
        ApartmentOrOfficeEntry.Text = _address.ApartmentOrOffice;
        EntranceEntry.Text = _address.Entrance;
        IntercomEntry.Text = _address.Intercom;
        CommentEditor.Text = _address.Comment;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        _address.City = CityEntry.Text;
        _address.Street = StreetEntry.Text;
        _address.House = HouseEntry.Text;
        _address.Floor = int.TryParse(FloorEntry.Text, out int floor) ? floor : null;
        _address.ApartmentOrOffice = ApartmentOrOfficeEntry.Text;
        _address.Entrance = EntranceEntry.Text;
        _address.Intercom = IntercomEntry.Text;
        _address.Comment = CommentEditor.Text;

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var jsonContent = new StringContent(JsonSerializer.Serialize(_address), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"http://course-project-4/api/addresses/{_address.Id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успех", "Адрес обновлен", "ОК");
                await Navigation.PopAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Код: {response.StatusCode}, Ответ: {errorContent}", "ОК");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "ОК");
        }
    }
}
