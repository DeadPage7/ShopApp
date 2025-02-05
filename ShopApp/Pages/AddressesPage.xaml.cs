using ShopApp.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;

namespace ShopApp.Pages;

public partial class AddressesPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private Client _client;
    private readonly string _token;

    public ObservableCollection<Address> Addresses { get; set; } = new ObservableCollection<Address>();

    public AddressesPage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;
        AddressesCollectionView.ItemsSource = Addresses;

        LoadAddresses();
    }

    private async void LoadAddresses()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync("http://course-project-4/api/addresses");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var addresses = JsonSerializer.Deserialize<List<Address>>(content);

                Addresses.Clear();
                foreach (var address in addresses)
                {
                    Addresses.Add(address);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Код: {response.StatusCode}, Ответ: {errorContent}", "ОК");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК");
        }
    }

    private async void OnCreateAddressClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateAddressPage(_client, _token));
    }

    private async void OnDeleteAddressClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Address selectedAddress)
        {
            bool confirm = await DisplayAlert("Подтверждение",
                                              $"Вы уверены, что хотите удалить адрес {selectedAddress.City}, {selectedAddress.Street}?",
                                              "Да", "Отмена");

            if (!confirm) return;

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.DeleteAsync($"http://course-project-4/api/addresses/{selectedAddress.Id}");

                if (response.IsSuccessStatusCode)
                {
                    Addresses.Remove(selectedAddress);
                    await DisplayAlert("Успех", "Адрес успешно удален!", "ОК");
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
    private async void OnAddressTapped(object sender, SelectionChangedEventArgs e)
    {
        // Получаем выбранного курьера
        if (e.CurrentSelection.FirstOrDefault() is Address selectedAddress)
        {
            // Переход на страницу информации о курьере
            await Navigation.PushAsync(new EditAddressPage(selectedAddress, _client, _token));
        }
    // Сбрасываем выбор
    ((CollectionView)sender).SelectedItem = null;
    }
}
